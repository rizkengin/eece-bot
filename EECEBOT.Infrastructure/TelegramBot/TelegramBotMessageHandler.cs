using System.Text;
using EECEBOT.Application.Common;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common;
using EECEBOT.Domain.Deadline;
using EECEBOT.Domain.Exam;
using EECEBOT.Domain.Link;
using EECEBOT.Domain.TelegramUser;
using Marten;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EECEBOT.Infrastructure.TelegramBot;

public class TelegramBotMessageHandler : ITelegramBotMessageHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IQuerySession _querySession;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TelegramBotMessageHandler(ITelegramBotClient botClient,
        IQuerySession querySession,
        IUnitOfWork unitOfWork,
        ITelegramUserRepository telegramUserRepository)
    {
        _botClient = botClient;
        _querySession = querySession;
        _unitOfWork = unitOfWork;
        _telegramUserRepository = telegramUserRepository;
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
        var exams = _querySession.Query<Exam>()
            .Where(x=> x.AcademicYear == user.AcademicYear)
            .ToList();

        if (exams.Count == 0)
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
        
        foreach (var exam in exams)
        {
            examsMessage.Append($"<b>{exam.Name}</b>\n");
            examsMessage.Append($"<b>Date:</b> {exam.Date}\n");
            examsMessage.Append($"<b>Location:</b> {exam.Location}\n\n");
        }
        
        await _botClient.SendTextMessageAsync(message.Chat.Id,
            examsMessage.ToString(),
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: cancellationToken);
    }

    public async Task HandleDeadlinesCommand(TelegramUser user, Message message, CancellationToken cancellationToken)
    {
        var deadlines = _querySession.Query<Deadline>()
            .Where(x=> x.AcademicYear == user.AcademicYear)
            .ToList();

        if (deadlines.Count == 0)
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
        
        foreach (var deadline in deadlines)
        {
            deadlinesMessage.Append($"<b>{deadline.Title}</b>\n");
            deadlinesMessage.Append($"<b>Description:</b> {deadline.Description}\n");
            deadlinesMessage.Append($"<b>Due Date:</b> {deadline.DueDate}\n\n");
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
        var links = _querySession.Query<Link>()
            .Where(x=> x.AcademicYear == user.AcademicYear)
            .ToList();

        if (links.Count == 0)
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
        
        foreach (var link in links)
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