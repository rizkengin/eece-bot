using System.Text;
using EECEBOT.Application.Common;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common;
using EECEBOT.Domain.TelegramUserAggregate;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EECEBOT.Infrastructure.TelegramBot;

public class TelegramBotMessageHandler : ITelegramBotMessageHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TelegramBotMessageHandler(
        ITelegramBotClient botClient,
        IAcademicYearRepository academicYearRepository,
        ITimeService timeService,
        ITelegramUserRepository telegramUserRepository,
        IUnitOfWork unitOfWork)
    {
        _botClient = botClient;
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
        _telegramUserRepository = telegramUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleStartFlow(Message message, CancellationToken cancellationToken)
    {
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
            
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Welcome to EECE BOT! Please select your academic year from the keyboard below. 🎓</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        
        await _botClient.SendStickerAsync(message.Chat.Id,
            new InputFileId(TelegramStickers.WelcomeSticker),
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingSectionFlow(Message message, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] {"Section 1"},
            new KeyboardButton[] {"Section 2"},
            new KeyboardButton[] {"Section 3"},
            new KeyboardButton[] {"Section 4"}
        })
        {
            ResizeKeyboard = true
        };
            
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Please select your section from the keyboard below. 🎓</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingBenchNumberFlow(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Please enter your bench number. 🎓</b>",
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingAcademicYearError(Message message, CancellationToken cancellationToken)
    {
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
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Wrong Input! Please select your study year from the keyboard below.</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingSectionFlowError(Message message, CancellationToken cancellationToken)
    {
        var keyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] {"Section 1"},
            new KeyboardButton[] {"Section 2"},
            new KeyboardButton[] {"Section 3"},
            new KeyboardButton[] {"Section 4"}
        })
        {
            ResizeKeyboard = true
        };
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Wrong Input! Please select your section from the keyboard below.</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingBenchNumberFlowError(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Wrong Input! Please enter your bench number.</b>",
            parseMode: ParseMode.Html,
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    public async Task HandleScheduleCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Normal Schedule", TelegramCallbackQueryData.NormalSchedule),
                InlineKeyboardButton.WithCallbackData("Lab Schedule", TelegramCallbackQueryData.LabSchedule)
            }
        });
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleExamsCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var exams = await _academicYearRepository
            .GetExamsAsync(user.Year, cancellationToken);
        
        if (exams.Value.ToList().Count == 0)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>You have no exams scheduled yet!</b>",
                replyMarkup: new ReplyKeyboardRemove(),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(message.Chat.Id,
                new InputFileId(TelegramStickers.HappyDogSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var examsMessage = new StringBuilder();
        
        foreach (var exam in exams.Value)
        {
            var timeLeft = exam.GetTimeLeft();
            
            examsMessage.Append($"<b><u>{exam.Name} ({exam.Type.ToString()})</u></b>\n");
            examsMessage.Append($"<b><u>Date:</u> {_timeService.ConvertUtcDateTimeOffsetToAppDateTime(exam.Date):dd-MM-yyy HH:mm}</b>\n");
            examsMessage.Append($"<b><u>Description:</u> {exam.Description}</b>\n");
            examsMessage.Append($"<b><u>Location:</u> {exam.Location ?? "TBD"}</b>\n");
            examsMessage.Append($"<b><u>Time Left:</u> {timeLeft.Days} days, {timeLeft.Hours} hours, {timeLeft.Minutes} minutes</b>\n\n");
        }
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            examsMessage.ToString(),
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleDeadlinesCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var deadlines = await _academicYearRepository
            .GetDeadlinesAsync(user.Year, cancellationToken);

        if (deadlines.Value
                .Where(x => x.DueDate >= _timeService.GetCurrentUtcTime())
                .ToList()
                .Count == 0)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>You have no deadlines yet!</b>",
                replyMarkup: new ReplyKeyboardRemove(),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(message.Chat.Id,
                new InputFileId(TelegramStickers.HappyDogSticker),
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var deadlinesMessage = new StringBuilder();
        
        foreach (var deadline in deadlines.Value)
        {
            var timeLeft = deadline.GetTimeLeft();
            
            deadlinesMessage.Append($"<b><u>{deadline.Title}</u></b>\n");
            deadlinesMessage.Append($"<b><u>Description:</u> {deadline.Description}</b>\n");
            deadlinesMessage.Append($"<b><u>Due Date:</u> {_timeService.ConvertUtcDateTimeOffsetToAppDateTime(deadline.DueDate):dd-MM-yyy HH:mm}</b>\n");
            deadlinesMessage.Append($"<b><u>Time Left:</u> {timeLeft.Days} days, {timeLeft.Hours} hours, {timeLeft.Minutes} minutes</b>\n\n");
        }
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            deadlinesMessage.ToString(),
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleHelpCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            $"<b>Hello {user.FirstName}, 👋\n\n<u>EECE BOT</u> is a Telegram bot that helps you keep track of your schedule, exams, deadlines and much more!\n\n" +
            "To get started, please select one of the following commands:\n\n" +
            "/schedule - to view your schedule. 📄\n" +
            "/exams - to view your exams. 📃\n" +
            "/deadlines - to view your deadlines. ⛔\n" +
            "/links - to view useful links. 🔗\n" +
            "/reset - to reset your academic year. 🔄\n" +
            "/help - to view this message again. 🆘\n\n" +
            "If you have any questions or suggestions, please contact @rizkengin 💖</b>",
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleLinksCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var links = await _academicYearRepository
            .GetLinksAsync(user.Year, cancellationToken);

        if (links.Value.ToList().Count == 0)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>There are no links available for your academic year yet!</b>",
                replyMarkup: new ReplyKeyboardRemove(),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(message.Chat.Id,
                new InputFileId(TelegramStickers.HappyDogSticker),
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }
        
        var linksMessage = new StringBuilder();
        
        foreach (var link in links.Value)
        {
            linksMessage.Append($"<b>{link.Name}</b>\n");
            linksMessage.Append($"<b>Link:</b> {link.Url}\n\n");
        }
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            linksMessage.ToString(),
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleUnknownInput(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Unknown input! Please try the /help command to view the list of available commands.</b>",
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleResetCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        _telegramUserRepository.ResetAcademicYear(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Your academic year has been reset successfully! 🎉</b>",
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        
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
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Please select your academic year. 🎓</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}