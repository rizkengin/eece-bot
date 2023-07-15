using System.Text;
using EECEBOT.Application.Common;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;
using EECEBOT.Domain.Common.TelegramBotIds;
using EECEBOT.Domain.TelegramUserAggregate;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EECEBOT.Infrastructure.TelegramBot;

public class TelegramBotCallbackQueryDataHandler : ITelegramBotCallbackQueryDataHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;

    public TelegramBotCallbackQueryDataHandler(
        ITelegramBotClient botClient,
        IAcademicYearRepository academicYearRepository,
        ITimeService timeService)
    {
        _botClient = botClient;
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
    }

    public async Task HandleNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Today's Schedule", TelegramCallbackQueryData.TodayNormalSchedule),
                InlineKeyboardButton.WithCallbackData("Tomorrow's Schedule", TelegramCallbackQueryData.TomorrowNormalSchedule)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Schedule file", TelegramCallbackQueryData.NormalScheduleFile)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Main Menu", TelegramCallbackQueryData.ScheduleMainMenu)
            }
        });
        
        await _botClient.EditMessageTextAsync(user.ChatId,
            callbackQuery.Message!.MessageId,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    
    public async Task HandleTodayNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var schedule = await _academicYearRepository.GetScheduleAsync(user.Year, cancellationToken);
        
        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        
        if (schedule.Value is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule is not yet updated by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var currentTime = _timeService.ConvertUtcDateTimeOffsetToAppDateTime(_timeService.GetCurrentUtcTime());

        var currentWeekType = schedule.Value.GetWeekType(DateOnly.FromDateTime(_timeService.GetCurrentUtcTime().DateTime));

        var todaySessions = schedule.Value.Sessions
            .Where(s => s.Sections.Contains((Section)user.Section!)
                        && s.DayOfWeek == currentTime.DayOfWeek
                        && (s.Frequency == SessionFrequency.Always || s.Frequency == currentWeekType.ToSessionFrequency()))
            .OrderBy(d => d.Period)
            .ToList();

        if (!todaySessions.Any())
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>You don't have any sessions today. 🎉</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.HappyDogSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var message = new StringBuilder();

        foreach (var session in todaySessions)
        {
            var subject = schedule.Value.Subjects.Single(s => s.Id == session.SubjectId);
            
            message.Append($"<b><u>Period</u> {session.Period.ToFriendlyString()}</b>\n");
            message.Append($"<b><u>Subject:</u> {subject.Name} ({subject.Code})</b>\n");
            message.Append($"<b><u>Location:</u> {session.Location}</b>\n");
            message.Append($"<b><u>Lecturer:</u> {session.Lecturer}</b>\n");
            message.Append($"<b><u>Session Type:</u> {session.SessionType.ToString()}</b>\n\n");
        }
        
        await _botClient.SendTextMessageAsync(user.ChatId,
            message.ToString(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    
    public async Task HandleTomorrowNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var schedule = await _academicYearRepository.GetScheduleAsync(user.Year, cancellationToken);
        
        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

        if (schedule.Value is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule is not yet updated by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var currentTime = _timeService.ConvertUtcDateTimeOffsetToAppDateTime(_timeService.GetCurrentUtcTime());

        var currentWeekType = schedule.Value.GetWeekType(DateOnly.FromDateTime(_timeService.GetCurrentUtcTime().DateTime));

        var tomorrowSessions = schedule.Value.Sessions
            .Where(s => s.Sections.Contains((Section)user.Section!)
                        && s.DayOfWeek == currentTime.AddDays(1).DayOfWeek
                        && (s.Frequency == SessionFrequency.Always || s.Frequency == currentWeekType.ToSessionFrequency()))
            .OrderBy(d => d.Period)
            .ToList();

        if (!tomorrowSessions.Any())
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>You don't have any sessions today. 🎉</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.HappyDogSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var message = new StringBuilder();

        foreach (var session in tomorrowSessions)
        {
            var subject = schedule.Value.Subjects.Single(s => s.Id == session.SubjectId);

            message.Append($"<b><u>Period</u> {session.Period.ToFriendlyString()}</b>\n");
            message.Append($"<b><u>Subject:</u> {subject.Name} ({subject.Code})</b>\n");
            message.Append($"<b><u>Location:</u> {session.Location}</b>\n");
            message.Append($"<b><u>Lecturer:</u> {session.Lecturer}</b>\n");
            message.Append($"<b><u>Session Type:</u> {session.SessionType.ToString()}</b>\n\n");
        }
        
        await _botClient.SendTextMessageAsync(user.ChatId,
            message.ToString(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    
    public async Task HandleNormalScheduleFileDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var schedule = await _academicYearRepository.GetScheduleAsync(user.Year, cancellationToken);
        
        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        
        if (schedule.Value is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule is not yet updated by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }

        if (schedule.Value.FileUri is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule file is not yet uploaded by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        await _botClient.SendChatActionAsync(user.ChatId,
            ChatAction.UploadDocument,
            cancellationToken: cancellationToken);
        
        await _botClient.SendDocumentAsync(user.ChatId,
            new InputFileUrl(schedule.Value.FileUri),
            cancellationToken: cancellationToken);
    }

    public async Task HandleLabScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("My Next Lab", TelegramCallbackQueryData.MyNextLabSchedule),
                InlineKeyboardButton.WithCallbackData("Lab Schedule File", TelegramCallbackQueryData.LabScheduleFile) 
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Main Menu 🔙", TelegramCallbackQueryData.ScheduleMainMenu)
            }
        });
        
        await _botClient.EditMessageTextAsync(user.ChatId,
            callbackQuery.Message!.MessageId,
            "<b>Please choose an option below</b>",
            parseMode: ParseMode.Html,
            replyMarkup: keyboard,
            cancellationToken: cancellationToken);
    }

    public async Task HandleScheduleMainMenuDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Normal Schedule", TelegramCallbackQueryData.NormalSchedule),
                InlineKeyboardButton.WithCallbackData("Lab Schedule", TelegramCallbackQueryData.LabSchedule)
            }
        });

        await _botClient.EditMessageTextAsync(user.ChatId,
            callbackQuery.Message!.MessageId,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    
    public async Task HandleMyNextLabDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var labSchedule = await _academicYearRepository.GetLabScheduleAsync(user.Year, cancellationToken);

        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        
        if (labSchedule.Value is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule is not yet updated by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);

            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }

        Lab? nextLab;

        TimeSpan nextLabEta;
        if (labSchedule.Value.SplitMethod is SplitMethod.ByBenchNumber)
        {
            nextLab = labSchedule.Value.Labs
                .Where(l => l.Section == user.Section && user.BenchNumber >= l.BenchNumbersRange!.Value.Start.Value
                                                      && user.BenchNumber <= l.BenchNumbersRange.Value.End.Value
                                                      && l.Date >= _timeService.GetCurrentUtcTime())
                .OrderBy(d => d.Date)
                .FirstOrDefault();

            if (nextLab is null)
            {
                await _botClient.SendTextMessageAsync(user.ChatId,
                    "<b>You don't have any labs left for this academic year.</b>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
                
                await _botClient.SendStickerAsync(user.ChatId,
                    new InputFileId(TelegramStickers.NoScheduleSticker),
                    cancellationToken: cancellationToken);
                
                return;
            }
            
            nextLabEta = nextLab.Date - _timeService.GetCurrentUtcTime();
            await _botClient.SendTextMessageAsync(user.ChatId,
                $"<b><u>Your next lab is:<u> {nextLab.Name}</b>\n" +
                $"<b><u>Lab date:<u> {_timeService.ConvertUtcDateTimeOffsetToAppDateTime(nextLab.Date):U}</b>\n" +
                $"<b><u>The location is:<u> {nextLab.Location}</b>\n" +
                $"<b><u>Remaining time:<u> {nextLabEta.Days} days {nextLabEta.Hours} hours {nextLabEta.Minutes} minutes</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);

            return;
        }
        
        nextLab = labSchedule.Value.Labs
            .Where(l => l.Section == user.Section && l.Date >= _timeService.GetCurrentUtcTime())
            .OrderBy(d => d.Date)
            .FirstOrDefault();

        if (nextLab is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>You don't have any labs left for this academic year.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
                
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
                
            return;
        }
        
        nextLabEta = nextLab.Date - _timeService.GetCurrentUtcTime();
        await _botClient.SendTextMessageAsync(user.ChatId,
            $"<b><u>Your next lab is:<u> {nextLab.Name}</b>\n" +
            $"<b><u>Lab date:<u> {_timeService.ConvertUtcDateTimeOffsetToAppDateTime(nextLab.Date):U}</b>\n" +
            $"<b><u>The location is:<u> {nextLab.Location}</b>\n" +
            $"<b><u>Remaining time:<u> {nextLabEta.Days} days {nextLabEta.Hours} hours {nextLabEta.Minutes} minutes</b>",
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
    
    public async Task HandleLabScheduleFileDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken)
    {
        var labSchedule = await _academicYearRepository.GetLabScheduleAsync(user.Year, cancellationToken);

        await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);
        
        if (labSchedule.Value is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule is not yet updated by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);

            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }

        if (labSchedule.Value.FileUri is null)
        {
            await _botClient.SendTextMessageAsync(user.ChatId,
                "<b>Schedule file is not yet uploaded by your academic year representative. Please be patient.</b>",
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(user.ChatId,
                new InputFileId(TelegramStickers.NoScheduleSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        await _botClient.SendChatActionAsync(user.ChatId,
            ChatAction.UploadDocument,
            cancellationToken: cancellationToken);
        
        await _botClient.SendDocumentAsync(user.ChatId,
            new InputFileUrl(labSchedule.Value.FileUri),
            cancellationToken: cancellationToken);
    }
}