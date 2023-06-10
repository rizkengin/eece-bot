using EECEBOT.Application.Common;
using EECEBOT.Application.Common.TelegramBot;
using MediatR;

namespace EECEBOT.Application.TelegramBot.Queries.CallbackQueryQuery;

public class CallbackQueryQueryHandler : IRequestHandler<CallbackQueryQuery>
{
    private readonly ITelegramBotCallbackQueryDataHandler _telegramBotCallbackQueryDataHandler;

    public CallbackQueryQueryHandler(ITelegramBotCallbackQueryDataHandler telegramBotCallbackQueryDataHandler)
    {
        _telegramBotCallbackQueryDataHandler = telegramBotCallbackQueryDataHandler;
    }

    public async Task Handle(CallbackQueryQuery request, CancellationToken cancellationToken)
    {
        var handler = request.CallbackQuery.Data switch
        {
            TelegramCallbackQueryData.NormalScheduleMainMenu => _telegramBotCallbackQueryDataHandler.HandleMainMenuDataAsync(request.CallbackQuery, cancellationToken),
            TelegramCallbackQueryData.NormalSchedule => _telegramBotCallbackQueryDataHandler.HandleNormalScheduleDataAsync(request.CallbackQuery, cancellationToken),
            TelegramCallbackQueryData.LabSchedule => _telegramBotCallbackQueryDataHandler.HandleLabScheduleDataAsync(request.CallbackQuery, cancellationToken),
            TelegramCallbackQueryData.GroupA => _telegramBotCallbackQueryDataHandler.HandleGroupADataAsync(request.CallbackQuery, cancellationToken),
            TelegramCallbackQueryData.GroupB => _telegramBotCallbackQueryDataHandler.HandleGroupBDataAsync(request.CallbackQuery, cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(request), request.CallbackQuery.Data, null)
        };
        
        await handler;
    }
}