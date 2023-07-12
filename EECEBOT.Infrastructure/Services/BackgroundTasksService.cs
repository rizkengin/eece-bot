﻿using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Interfaces;
using EECEBOT.Domain.TelegramUserAggregate;
using EECEBOT.Infrastructure.Persistence;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
                _logger.LogError(e, "Unable to publish domain event {DomainEvent} from outbox message {MessageId}", domainEvent.GetType().Name, outboxMessage.Id);
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
                    new InputFileUrl("https://cdn.discordapp.com/attachments/923250094755696651/1127219409090515077/Screenshot_2023-07-08_154401.png"),
                    caption: $"<b>Hello {user.FirstName},\n" +
                             """If you like this bot, please consider giving the <a href="https://github.com/rizkengin/eece-bot">project repo</a> a star ⭐ on github.""" +
                             "\nThis will help keep the project alive and maintained. ❤️</b>\n",
                    parseMode: ParseMode.Html))
            .Cast<Task>()
            .ToList();

        await Task.WhenAll(tasks);
    }
}