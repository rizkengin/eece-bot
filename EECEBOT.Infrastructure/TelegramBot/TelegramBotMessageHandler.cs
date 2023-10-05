using System.Text;
using EECEBOT.Application.Common;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common.TelegramBotIds;
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
            "<b>Welcome to EECE BOT! Please select your academic year from the keyboard below. 🎓\n\n</b>" +
            "<i><u>Note:</u> You can reset your academic year at any time later on.</i>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
        
        await _botClient.SendStickerAsync(message.Chat.Id,
            new InputFileId(TelegramStickers.HiFireSticker),
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
            "<b>Please select your section from the keyboard below. 🎓\n\n</b>"+
            "<i><u>Note:</u> You can reset your section at any time later on.</i>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandlePickingBenchNumberFlow(Message message, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            "<b>Please enter your bench number. 🎓\n\n</b>" +
            "<i><u>Note:</u> You can reset your bench number at any time later on.</i>",
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
        
        var upcomingExams = exams.IsError ? 
            null : 
            exams.Value
                .Where(e => e.Date > _timeService.GetCurrentUtcTime())
                .OrderBy(e => e.Date)
                .ToList();
        
        if (upcomingExams is null || upcomingExams.Count == 0)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>There are no upcoming exams. 📚</b>",
                replyMarkup: new ReplyKeyboardRemove(),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(message.Chat.Id,
                new InputFileId(TelegramStickers.HappyDogSticker),
                cancellationToken: cancellationToken);
            
            return;
        }
        
        var examsMessage = new StringBuilder();
        
        foreach (var exam in upcomingExams)
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

        var upcomingDeadlines = deadlines.IsError ?
            null :
            deadlines.Value
                .Where(x => x.DueDate >= _timeService.GetCurrentUtcTime())
                .OrderBy(x => x.DueDate)
                .ToList();
        
        if (upcomingDeadlines is null || upcomingDeadlines.Count == 0)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>There are no upcoming deadlines. 📚</b>",
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
        
        foreach (var deadline in upcomingDeadlines)
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
            $"<b>Hello {user.FirstName}, 👋\n\n<u><i>EECE BOT</i></u> is a telegram bot that helps you keep track of your schedule, exams, deadlines and much more!\n\n" +
            "To get started you can watch this <a href=\"https://youtu.be/iirNy6srqB8\">demo</a> or please select one of the following commands:\n\n" +
            "/schedule - to view your schedule. 📄\n" +
            "/exams - to view your exams. 📃\n" +
            "/deadlines - to view your deadlines. ⛔\n" +
            "/links - to view useful links. 🔗\n" +
            "/reset - to reset your academic year and bench number. 🔄\n" +
            "/help - to view this message again. 🆘\n\n" +
            "If you have any questions or suggestions, please contact @rizkengin 💖\n\n" +
            "<u>البوت ممكن يساعدك ازاي؟ 🕺\n\n</u>" +
            "🌟 هيساعدك توصل لجدولك بتاع النهارده و بكره بكل سهوله وبيبعتلك ملف الجدول كامل لو احتجته.\n" +
            "🌟هيساعدك تعرف معملك الجاي ومكانه فين بكل سهوله من غير متحتاج الجدول وكمان هيبعتلك الجدول كامل لو احتجته.\n" +
            "🌟 هيساعدك تعرف مواعيد كل الكويزات و الأمتحانات بتاعتك في اي وقت وكمان هيبعتلك التفاصيل اللي محتاج تعرفها عن كل امتحان منهم.\n" +
            "🌟 هيساعدك تعرف كل الديدلاينز اللي عليك الفتره الجايه من بروجيكات او ابحاث او غيره بكل سهوله في اي وقت.\n" +
            "🌟 هيساعدك توصل لكل اللينكات المهمه بتاعة السنه الدراسية اللي انت فيها سواء لينك الدرايف بتاعك او غيره.\n" +
            "🌟 هيبعتلك تنبيه اول ما النتايج تبدأ تظهر و هيبعتلك اشعار اول ما النتيجة بتاعتك تظهر.\n\n" +
            "كل اللي عليك انك تستخدم الأوامر اللي مكتوبه فوق الخاصه بكل حاجه من الحجات دي. 👏🚀</b>",
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleLinksCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var links = await _academicYearRepository
            .GetLinksAsync(user.Year, cancellationToken);

        if (links.IsError || !links.Value.Any())
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id,
                "<b>There are no links available. 📚</b>",
                replyMarkup: new ReplyKeyboardRemove(),
                parseMode: ParseMode.Html,
                cancellationToken: cancellationToken);
            
            await _botClient.SendStickerAsync(message.Chat.Id,
                new InputFileId(TelegramStickers.HappyDogSticker),
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
            
            return;
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
            "<b>Please select your academic year. 🎓\n\n</b>" +
            "<i><u>Note:</u> You can reset your academic year at any time later on.</i>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}