using Telegram.Bot.Types;

namespace EECEBOT.Application.Common.TelegramBot;

public interface ITelegramBotCallbackQueryDataHandler
{
    Task HandleNormalScheduleDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
    Task HandleLabScheduleDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
    Task HandleMainMenuDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
    Task HandleGroupADataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
    Task HandleGroupBDataAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}