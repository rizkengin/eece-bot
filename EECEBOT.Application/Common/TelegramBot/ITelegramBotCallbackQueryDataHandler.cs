using EECEBOT.Domain.TelegramUser;
using Telegram.Bot.Types;

namespace EECEBOT.Application.Common.TelegramBot;

public interface ITelegramBotCallbackQueryDataHandler
{
    Task HandleNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleTodayNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleTomorrowNormalScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleNormalScheduleFileDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleLabScheduleDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleScheduleMainMenuDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleMyNextLabDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
    Task HandleLabScheduleFileDataAsync(CallbackQuery callbackQuery, TelegramUser user, CancellationToken cancellationToken);
}