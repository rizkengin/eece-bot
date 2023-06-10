using EECEBOT.Application.Common;
using EECEBOT.Application.Common.TelegramBot;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EECEBOT.Infrastructure.TelegramBot;

public class TelegramBotCallbackQueryDataHandler : ITelegramBotCallbackQueryDataHandler
{
    private readonly ITelegramBotClient _botClient;

    public TelegramBotCallbackQueryDataHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task HandleNormalScheduleDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Group A", TelegramCallbackQueryData.GroupA),
                InlineKeyboardButton.WithCallbackData("Group B", TelegramCallbackQueryData.GroupB),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Main Menu", TelegramCallbackQueryData.NormalScheduleMainMenu),
            }
        });
        
        await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            "<b>Please choose your group. 🧑‍🤝‍🧑</b>", 
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleLabScheduleDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Group A", TelegramCallbackQueryData.GroupALab),
                InlineKeyboardButton.WithCallbackData("Group B", TelegramCallbackQueryData.GroupBLab), 
            }
        });
        
        await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            "<b>Please choose your group. 🧑‍🤝‍🧑</b>", 
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleMainMenuDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new []
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData("Normal Schedule", TelegramCallbackQueryData.NormalSchedule),
                InlineKeyboardButton.WithCallbackData("Lab Schedule", TelegramCallbackQueryData.LabSchedule)
            }
        });

        await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleGroupADataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Today's Schedule", TelegramCallbackQueryData.GroupATodaySchedule),
                InlineKeyboardButton.WithCallbackData("Tomorrow's Schedule", TelegramCallbackQueryData.GroupATomorrowSchedule)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Schedule file", TelegramCallbackQueryData.NormalScheduleFile)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Main Menu", TelegramCallbackQueryData.NormalScheduleMainMenu)
            }
        });
        
        await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }

    public async Task HandleGroupBDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Today's Schedule",
                    TelegramCallbackQueryData.GroupBTodaySchedule),
                InlineKeyboardButton.WithCallbackData("Tomorrow's Schedule",
                    TelegramCallbackQueryData.GroupBTomorrowSchedule),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Schedule file", TelegramCallbackQueryData.NormalScheduleFile)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("🔙 Main Menu", TelegramCallbackQueryData.NormalScheduleMainMenu)
            }
        });
        
        await _botClient.EditMessageTextAsync(callbackQuery.Message!.Chat.Id,
            callbackQuery.Message.MessageId,
            "<b>Please select the schedule you want to view. 📄</b>",
            replyMarkup: keyboard,
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}