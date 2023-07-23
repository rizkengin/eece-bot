using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Interfaces;
using EECEBOT.Domain.Common.TelegramBotIds;
using EECEBOT.Domain.TelegramUserAggregate;
using EECEBOT.Infrastructure.Persistence;
using EECEBOT.Infrastructure.Services.AcademicYearsResults;
using Hangfire;
using HtmlAgilityPack;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = EECEBOT.Domain.UserAggregate.User;

namespace EECEBOT.Infrastructure.Services;

public class BackgroundTasksService : IBackgroundTasksService
{
    private readonly IDocumentSession _session;
    private readonly IPublisher _publisher;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly ILogger<BackgroundTasksService> _logger;

    public BackgroundTasksService(
        IDocumentSession session,
        IPublisher publisher,
        ITelegramBotClient telegramBotClient,
        ILogger<BackgroundTasksService> logger)
    {
        _session = session;
        _publisher = publisher;
        _telegramBotClient = telegramBotClient;
        _logger = logger;
    }
    
    [DisableConcurrentExecution(20)]
    public async Task ProcessOutboxMessagesAsync()
    {
        _logger.LogInformation("Processing outbox messages");
        
        var messages = await _session
            .Query<OutboxMessage>()
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync();

        foreach (var outboxMessage in messages)
        {
            var domainEvent = JsonConvert
                               .DeserializeObject<IDomainEvent>
                               (outboxMessage.Content,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.All
                                });

            if (domainEvent is null)
            {
                _logger.LogWarning("Unable to deserialize domain event from outbox message {MessageId}", outboxMessage.Id);
                continue;
            }
            
            try
            {
                await _publisher.Publish(domainEvent);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                        "Unable to publish domain event {DomainEvent} from outbox message {MessageId}",
                        domainEvent.GetType().Name,
                        outboxMessage.Id);
                
                outboxMessage.Error = e.Message;
            }
            
            outboxMessage.ProcessedOnUtc = DateTime.UtcNow;
        }

        _session.Update<OutboxMessage>(messages);

        await _session.SaveChangesAsync();
    }

    public async Task RequestGithubRepoStarFromUsersAsync()
    {
        _logger.LogInformation("Requesting github repo star from users");

        var users = await _session
            .Query<TelegramUser>()
            .Where(u => u.Year != Year.None)
            .ToListAsync();

        var tasks = users
            .Select(user => _telegramBotClient.
                SendPhotoAsync(
                    user.ChatId,
                    new InputFileUrl(TelegramFiles.GithubRepositoryStarHelperImage),
                    caption: $"<b>Hello {user.FirstName},\n" +
                             """If you like this bot, please consider giving the <a href="https://github.com/rizkengin/eece-bot">project repo</a> a star ⭐ on github.""" +
                             "\nThis will help keep the project alive and maintained. ❤️</b>\n",
                    parseMode: ParseMode.Html))
            .Cast<Task>()
            .ToList();

        await Task.WhenAll(tasks);
    }

    public async Task ExpiredRefreshTokensCleanupAsync()
    {
        _logger.LogInformation("Cleaning up expired refresh tokens");

        var users = await _session
            .Query<User>()
            .Where(u => u.RefreshTokens.Any(t => t.ExpiresOn < DateTime.UtcNow) ||
                        u.RefreshTokens.Any(t => t.IsUsed) ||
                        u.RefreshTokens.Any(t => t.IsInvalidated))
            .ToListAsync();
        
        foreach (var user in users)
            user.RefreshTokensCleanup();
        
        _session.Update<User>(users);
        
        await _session.SaveChangesAsync();
    }

    public async Task AcademicYearResetProcessAsync()
    {
        var activeTelegramUsers = await _session
            .Query<TelegramUser>()
            .Where(u => u.Year != Year.None)
            .ToListAsync();

        var telegramMessagesTasks = new List<Task>();
        
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] {"1st year"},
            new KeyboardButton[] {"2nd year"},
            new KeyboardButton[] {"3rd year"},
            new KeyboardButton[] {"4th year"}
        })
        {
            ResizeKeyboard = true
        };
        
        foreach (var telegramUser in activeTelegramUsers)
        {
            telegramUser.ResetAcademicYear();
            
            _session.Update(telegramUser);
            
            telegramMessagesTasks
                .Add(_telegramBotClient.SendTextMessageAsync(
                    telegramUser.ChatId,
                    "<b>Your academic year has been automatically reset!. Please select your academic year. 🎓</b>",
                    replyMarkup: keyboard,
                    parseMode: ParseMode.Html));
        }

        await _session.SaveChangesAsync();
        
        await Task.WhenAll(telegramMessagesTasks);
    }

    public async Task CheckForAcademicYearsResultsAsync()
    {
        var academicYearsCurrentResults = await _session
                                       .Query<AcademicYearResult>()
                                       .ToListAsync();

        var web = new HtmlWeb();

        var resultsHtmlPage = await web.LoadFromWebAsync("http://www.results.eng.cu.edu.eg/");
    }
}