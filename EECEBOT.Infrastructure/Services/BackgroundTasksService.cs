﻿using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Interfaces;
using EECEBOT.Domain.Common.TelegramBotIds;
using EECEBOT.Domain.TelegramUserAggregate;
using EECEBOT.Infrastructure.Persistence;
using EECEBOT.Infrastructure.Services.AcademicYearsResults;
using EECEBOT.Infrastructure.Services.AcademicYearsResults.Enums;
using Hangfire;
using HtmlAgilityPack;
using Marten;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
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

        foreach (var user in users)
        {
            try
            {
                await _telegramBotClient.
                SendPhotoAsync(
                    user.ChatId,
                    new InputFileUrl(TelegramFiles.GithubRepositoryStarHelperImage),
                    caption: "<b>اهلا اهلا 😁😁\n" +
                             "اتمني تكون تجربة استخدام البوت كويسه وفيها استفادة وتوفير وقت ليك/ي. 👏\n" +
                             """لو شايف انك مستفيد فعلا من البوت ياريت تاخد من وقتك دقيقه وتعمل ستار <a href="https://github.com/rizkengin/eece-bot">للريبو</a> الخاصه بالبوت. ⭐⭐""" +
                             "\nدا هيساعد ويشجع جدا علي ان البوت يفضل لايف ويتعمله تحديثات اول بـ اول. 🚀🚀\n" +
                             "وكمان هيساعد علي ان البوت يوصل لاكبر عدد ممكن من الطلاب ويفيد اكبر عدد ممكن. 🤝🤝\n" +
                             "وشكرا جدا ليك/ي. 🙏🙏</b>",
                    parseMode: ParseMode.Html);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to send github repo star request to user {ChatId}", user.ChatId);

                _session.Delete(user);

                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", user.ChatId, user.FirstName);
            }
        }

        await _session.SaveChangesAsync();
    }

    public async Task ExpiredRefreshTokensCleanupAsync()
    {
        _logger.LogInformation("Cleaning up expired refresh tokens");

        var users = _session
            .Query<User>()
            .AsEnumerable()
            .Where(u => u.RefreshTokens.Any(t => t.ExpiresOn < DateTimeOffset.UtcNow) ||
                        u.RefreshTokens.Any(t => t.IsUsed) ||
                        u.RefreshTokens.Any(t => t.IsInvalidated))
            .ToList();

        if (users.Count == 0)
            return;

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

        var academicYears = await _session
            .Query<AcademicYear>()
            .ToListAsync();

        var users = await _session
            .Query<User>()
            .ToListAsync();

        foreach (var academicYear in academicYears)
        {
            academicYear.Reset();

            _session.Update(academicYear);
        }

        foreach (var user in users)
        {
            user.ResetAccess();

            _session.Update(user);
        }

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

            try
            {
                await _telegramBotClient.SendTextMessageAsync(
                    telegramUser.ChatId,
                    "<b>Your academic year has been automatically reset!. Please select your academic year. 🎓</b>",
                    replyMarkup: keyboard,
                    parseMode: ParseMode.Html);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to send academic year reset message to user {ChatId}", telegramUser.ChatId);

                _session.Delete(telegramUser);

                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", telegramUser.ChatId, telegramUser.FirstName);
            }
        }

        await _session.SaveChangesAsync();
    }

    [DisableConcurrentExecution(20)]
    [AutomaticRetry(OnAttemptsExceeded = AttemptsExceededAction.Delete)]
    public async Task CheckForAcademicYearsResultsAsync()
    {
        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright
            .Chromium
            .LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true
                });

        var page = await browser.NewPageAsync();

        await page.GotoAsync("http://www.results.eng.cu.edu.eg/");

        var resultsHtmlPage = await page.ContentAsync();

        var htmlDocument = new HtmlDocument();

        htmlDocument.LoadHtml(resultsHtmlPage);

        var table = htmlDocument
            .DocumentNode
            .SelectSingleNode("""//*[@id="AutoNumber4"]""")
            .ChildNodes["tbody"]
            .ChildNodes[2];

        var isFirstYearResultAvailable = table
            .ChildNodes
            .First(x => x.Id == "td1")
            .ChildNodes
            .Count > 0;

        var isSecondYearResultAvailable = table
            .ChildNodes
            .First(x => x.Id == "td2")
            .ChildNodes
            .Count > 0;

        var isThirdYearResultAvailable = table
            .ChildNodes
            .First(x => x.Id == "td3")
            .ChildNodes
            .Count > 0;

        var isFourthYearResultAvailable = table
            .ChildNodes
            .First(x => x.Id == "td4")
            .ChildNodes
            .Count > 0;

        var academicYearResults = await _session
            .Query<AcademicYearResult>()
            .ToListAsync();

        var telegramUsers = await _session
            .Query<TelegramUser>()
            .ToListAsync();

        var firstYearLastResult = academicYearResults
            .SingleOrDefault(x => x.AcademicYear == Year.FirstYear);

        var secondYearLastResult = academicYearResults
            .SingleOrDefault(x => x.AcademicYear == Year.SecondYear);

        var thirdYearLastResult = academicYearResults
            .SingleOrDefault(x => x.AcademicYear == Year.ThirdYear);

        var fourthYearLastResult = academicYearResults
            .SingleOrDefault(x => x.AcademicYear == Year.FourthYear);

        if (firstYearLastResult is null)
        {
            firstYearLastResult = AcademicYearResult.Create(Year.FirstYear, isFirstYearResultAvailable ? ResultStatus.Available : ResultStatus.UnAvailable);

            _session.Store(firstYearLastResult);
        }
        else
        {
            switch (isFirstYearResultAvailable)
            {
                case true when firstYearLastResult.LastResultStatus == ResultStatus.UnAvailable:
                    {
                        firstYearLastResult.LastResultStatus = ResultStatus.Available;

                        var firstYearUsers = telegramUsers
                            .Where(u => u.Year == Year.FirstYear)
                            .ToList();

                        foreach (var firstYearUser in firstYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    firstYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نتيجة السنة الأولي ظهرت. بالتوفيق للجميع. 💐</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    firstYearUser.ChatId,
                                    new InputFileId(TelegramStickers.AnnouncementFireSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", firstYearUser.ChatId);

                                _session.Delete(firstYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", firstYearUser.ChatId, firstYearUser.FirstName);
                            }
                        }
                        
                        _session.Update(firstYearLastResult);
                        
                        break;
                    }
                case false when firstYearLastResult.LastResultStatus == ResultStatus.Available:
                    {
                        firstYearLastResult.LastResultStatus = ResultStatus.UnAvailable;

                        var firstYearUsers = telegramUsers
                            .Where(u => u.Year == Year.FirstYear)
                            .ToList();

                        foreach(var firstYearUser in firstYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    firstYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نصبوا الصوان خلاص. استعد لظهور النتيجة 😢</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    firstYearUser.ChatId,
                                    new InputFileId(TelegramStickers.WorriedDogSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", firstYearUser.ChatId);

                                _session.Delete(firstYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", firstYearUser.ChatId, firstYearUser.FirstName);
                            }
                        }

                        _session.Update(firstYearLastResult);
                        
                        break;
                    }
            }
        }

        if (secondYearLastResult is null)
        {
            secondYearLastResult = AcademicYearResult.Create(Year.SecondYear, isSecondYearResultAvailable ? ResultStatus.Available : ResultStatus.UnAvailable);

            _session.Store(secondYearLastResult);
        }
        else
        {
            switch (isSecondYearResultAvailable)
            {
                case true when secondYearLastResult.LastResultStatus == ResultStatus.UnAvailable:
                    {
                        secondYearLastResult.LastResultStatus = ResultStatus.Available;

                        var secondYearUsers = telegramUsers
                            .Where(u => u.Year == Year.SecondYear)
                            .ToList();

                        foreach(var secondYearUser in secondYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    secondYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نتيجة السنة الثانية ظهرت. بالتوفيق للجميع. 💐</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    secondYearUser.ChatId,
                                    new InputFileId(TelegramStickers.AnnouncementFireSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", secondYearUser.ChatId);

                                _session.Delete(secondYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", secondYearUser.ChatId, secondYearUser.FirstName);
                            }
                        }
                        
                        _session.Update(secondYearLastResult);
                        
                        break;
                    }
                case false when secondYearLastResult.LastResultStatus == ResultStatus.Available:
                    {
                        secondYearLastResult.LastResultStatus = ResultStatus.UnAvailable;

                        var secondYearUsers = telegramUsers
                            .Where(u => u.Year == Year.SecondYear)
                            .ToList();

                        foreach(var secondYearUser in secondYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    secondYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نصبوا الصوان خلاص. استعد لظهور النتيجة 😢</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    secondYearUser.ChatId,
                                    new InputFileId(TelegramStickers.WorriedDogSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", secondYearUser.ChatId);

                                _session.Delete(secondYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", secondYearUser.ChatId, secondYearUser.FirstName);
                            }
                        }
                        
                        _session.Update(secondYearLastResult);
                        
                        break;
                    }
            }
        }

        if (thirdYearLastResult is null)
        {
            thirdYearLastResult = AcademicYearResult.Create(Year.ThirdYear, isThirdYearResultAvailable ? ResultStatus.Available : ResultStatus.UnAvailable);

            _session.Store(thirdYearLastResult);
        }
        else
        {
            switch (isThirdYearResultAvailable)
            {
                case true when thirdYearLastResult.LastResultStatus == ResultStatus.UnAvailable:
                    {
                        thirdYearLastResult.LastResultStatus = ResultStatus.Available;

                        var thirdYearUsers = telegramUsers
                            .Where(u => u.Year == Year.ThirdYear)
                            .ToList();
                        
                        foreach(var thirdYearUser in thirdYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    thirdYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نتيجة السنة الثالثة ظهرت. بالتوفيق للجميع. 💐</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    thirdYearUser.ChatId,
                                    new InputFileId(TelegramStickers.AnnouncementFireSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", thirdYearUser.ChatId);

                                _session.Delete(thirdYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", thirdYearUser.ChatId, thirdYearUser.FirstName);
                            }
                        }

                        _session.Update(thirdYearLastResult);
                        
                        break;
                    }
                case false when thirdYearLastResult.LastResultStatus == ResultStatus.Available:
                    {
                        thirdYearLastResult.LastResultStatus = ResultStatus.UnAvailable;

                        var thirdYearUsers = telegramUsers
                            .Where(u => u.Year == Year.ThirdYear)
                            .ToList();

                        foreach(var thirdYearUser in thirdYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    thirdYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نصبوا الصوان خلاص. استعد لظهور النتيجة 😢</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    thirdYearUser.ChatId,
                                    new InputFileId(TelegramStickers.WorriedDogSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", thirdYearUser.ChatId);

                                _session.Delete(thirdYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", thirdYearUser.ChatId, thirdYearUser.FirstName);
                            }
                        }
                        
                        _session.Update(thirdYearLastResult);
                        
                        break;
                    }
            }
        }

        if (fourthYearLastResult is null)
        {
            fourthYearLastResult = AcademicYearResult.Create(Year.FourthYear, isFourthYearResultAvailable ? ResultStatus.Available : ResultStatus.UnAvailable);

            _session.Store(fourthYearLastResult);
        }
        else
        {
            switch (isFourthYearResultAvailable)
            {
                case true when fourthYearLastResult.LastResultStatus == ResultStatus.UnAvailable:
                    {
                        fourthYearLastResult.LastResultStatus = ResultStatus.Available;

                        var fourthYearUsers = telegramUsers
                            .Where(u => u.Year == Year.FourthYear)
                            .ToList();

                        foreach(var fourthYearUser in fourthYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    fourthYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نتيجة السنة الرابعة ظهرت. بالتوفيق للجميع. 💐</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    fourthYearUser.ChatId,
                                    new InputFileId(TelegramStickers.AnnouncementFireSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", fourthYearUser.ChatId);

                                _session.Delete(fourthYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", fourthYearUser.ChatId, fourthYearUser.FirstName);
                            }
                        }

                        _session.Update(fourthYearLastResult);
                        
                        break;
                    }
                case false when fourthYearLastResult.LastResultStatus == ResultStatus.Available:
                    {
                        fourthYearLastResult.LastResultStatus = ResultStatus.UnAvailable;

                        var fourthYearUsers = telegramUsers
                            .Where(u => u.Year == Year.FourthYear)
                            .ToList();

                        foreach(var fourthYearUser in fourthYearUsers)
                        {
                            try
                            {
                                await _telegramBotClient
                                .SendTextMessageAsync(
                                    fourthYearUser.ChatId,
                                    "🚨🚨🚨\n\n" +
                                    "<b>نصبوا الصوان خلاص. استعد لظهور النتيجة 😢</b>\n\n" +
                                    "🚨🚨🚨",
                                    parseMode: ParseMode.Html);
                                
                                await _telegramBotClient
                                .SendStickerAsync(
                                    fourthYearUser.ChatId,
                                    new InputFileId(TelegramStickers.WorriedDogSticker));
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e, "Unable to send academic year result notification to user {ChatId}", fourthYearUser.ChatId);

                                _session.Delete(fourthYearUser);

                                _logger.LogInformation("User with Id {ChatId} and Name {UserName} has been removed from the database", fourthYearUser.ChatId, fourthYearUser.FirstName);
                            }
                        }

                        _session.Update(fourthYearLastResult);
                        
                        break;
                    }
            }
        }

        await _session.SaveChangesAsync();

        _logger.LogInformation("Academic years results checked");
    }
}