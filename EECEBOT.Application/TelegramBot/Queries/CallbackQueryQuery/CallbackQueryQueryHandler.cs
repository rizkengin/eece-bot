using EECEBOT.Application.Common;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.TelegramUserAggregate;
using MediatR;
using Microsoft.Extensions.Logging;

namespace EECEBOT.Application.TelegramBot.Queries.CallbackQueryQuery;

public class CallbackQueryQueryHandler : IRequestHandler<CallbackQueryQuery>
{
    private readonly ITelegramBotCallbackQueryDataHandler _telegramBotCallbackQueryDataHandler;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly ITelegramBotMessageHandler _telegramBotMessageHandler;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CallbackQueryQueryHandler> _logger;

    public CallbackQueryQueryHandler(
        ITelegramBotCallbackQueryDataHandler telegramBotCallbackQueryDataHandler,
        ITelegramUserRepository telegramUserRepository,
        ITelegramBotMessageHandler telegramBotMessageHandler,
        IUnitOfWork unitOfWork,
        ILogger<CallbackQueryQueryHandler> logger)
    {
        _telegramBotCallbackQueryDataHandler = telegramBotCallbackQueryDataHandler;
        _telegramUserRepository = telegramUserRepository;
        _telegramBotMessageHandler = telegramBotMessageHandler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(CallbackQueryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CallbackQuery received from chatId: {ChatId} and name {FirstName} with data: {Data}",
            request.CallbackQuery.Message?.Chat.Id,
            request.CallbackQuery.From.FirstName,
            request.CallbackQuery.Data);
        
        var user = await _telegramUserRepository.GetByTelegramIdAsync(request.CallbackQuery.From.Id, cancellationToken);

        if (user is null)
        {
            user = TelegramUser.Create(
                request.CallbackQuery.From.FirstName,
                request.CallbackQuery.From.Id,
                request.CallbackQuery.Message!.Chat.Id,
                request.CallbackQuery.From.LastName,
                request.CallbackQuery.From.Username);
            
            _telegramUserRepository.Create(user);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _telegramBotMessageHandler.HandleStartFlow(request.CallbackQuery.Message, cancellationToken);
            
            return;
        }

        if (request.CallbackQuery.Data is null)
            return;

        if (user.CurrentState != CurrentState.None)
        {
            await _telegramBotMessageHandler.HandleUnknownInput(request.CallbackQuery.Message!, cancellationToken);
            
            return;
        }

        var handler = request.CallbackQuery.Data switch
        {
            TelegramCallbackQueryData.NormalSchedule => _telegramBotCallbackQueryDataHandler
                .HandleNormalScheduleDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.TodayNormalSchedule => _telegramBotCallbackQueryDataHandler
                .HandleTodayNormalScheduleDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.TomorrowNormalSchedule => _telegramBotCallbackQueryDataHandler
                .HandleTomorrowNormalScheduleDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.LabSchedule => _telegramBotCallbackQueryDataHandler
                .HandleLabScheduleDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.ScheduleMainMenu => _telegramBotCallbackQueryDataHandler
                .HandleScheduleMainMenuDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.MyNextLabSchedule => _telegramBotCallbackQueryDataHandler
                .HandleMyNextLabDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.LabScheduleFile => _telegramBotCallbackQueryDataHandler
                .HandleLabScheduleFileDataAsync(request.CallbackQuery, user, cancellationToken),
            TelegramCallbackQueryData.NormalScheduleFile => _telegramBotCallbackQueryDataHandler
                .HandleNormalScheduleFileDataAsync(request.CallbackQuery, user, cancellationToken),
            _ => _telegramBotMessageHandler.HandleUnknownInput(request.CallbackQuery.Message!, cancellationToken)
        };

        await handler;
    }
}