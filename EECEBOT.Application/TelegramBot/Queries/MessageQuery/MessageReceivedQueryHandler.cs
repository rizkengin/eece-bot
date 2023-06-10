using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.TelegramUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace EECEBOT.Application.TelegramBot.Queries.MessageQuery;

public class MessageReceivedQueryHandler : IRequestHandler<MessageReceivedQuery>
{
    private readonly ILogger<MessageReceivedQueryHandler> _logger;
    private readonly ITelegramBotMessageHandler _telegramBotMessageHandler;
    private readonly ITelegramUserRepository _telegramUserRepository;

    public MessageReceivedQueryHandler(ILogger<MessageReceivedQueryHandler> logger,
        ITelegramBotMessageHandler telegramBotMessageHandler,
        ITelegramUserRepository telegramUserRepository)
    {
        _logger = logger;
        _telegramBotMessageHandler = telegramBotMessageHandler;
        _telegramUserRepository = telegramUserRepository;
    }

    public async Task Handle(MessageReceivedQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", request.Message.Type);

        var user = await _telegramUserRepository.GetByChatIdAsync(request.Message.Chat.Id, cancellationToken);
        
        if (user is null)
        {
            user = TelegramUser.Create(request.Message.From!.FirstName, request.Message.From.LastName,
                request.Message.From.Username, request.Message.Chat.Id);
            
            await _telegramUserRepository.CreateAsync(user, cancellationToken);

            await _telegramBotMessageHandler.HandleStartFlow(request.Message, cancellationToken);
            
            return;
        }

        if (user.CurrentState == CurrentState.PickingStudyYear)
        {
            var handlePickingStudyYear = request.Message.Text switch
            {
                "1st year" => _telegramUserRepository.UpdateStudyYearAsync(user, StudyYear.First,
                    cancellationToken),
                "2nd year" => _telegramUserRepository.UpdateStudyYearAsync(user, StudyYear.Second,
                    cancellationToken),
                "3rd year" => _telegramUserRepository.UpdateStudyYearAsync(user, StudyYear.Third,
                    cancellationToken),
                "4th year" => _telegramUserRepository.UpdateStudyYearAsync(user, StudyYear.Fourth,
                    cancellationToken),
                _ => _telegramBotMessageHandler.HandlePickingStudyYearError(request.Message, cancellationToken)
            };
            
            await handlePickingStudyYear;
            
            if(request.Message.Text is "1st year" or "2nd year" or "3rd year" or "4th year")
                await _telegramBotMessageHandler.HandleHelpCommand(user, request.Message, cancellationToken);
            
            return;
        }

        if (request.Message.Type != MessageType.Text)
        {
            await _telegramBotMessageHandler.HandleUnknownInput(request.Message, cancellationToken);
            return;
        }

        var handleTextMessage = request.Message.Text switch
        {
            "/schedule" => _telegramBotMessageHandler.HandleScheduleCommand(user, request.Message, cancellationToken),
            "/exams" => _telegramBotMessageHandler.HandleExamsCommand(user, request.Message, cancellationToken),
            "/deadlines" => _telegramBotMessageHandler.HandleDeadlinesCommand(user, request.Message, cancellationToken),
            "/links" => _telegramBotMessageHandler.HandleLinksCommand(user, request.Message, cancellationToken),
            "/help" => _telegramBotMessageHandler.HandleHelpCommand(user, request.Message, cancellationToken),
            _ => _telegramBotMessageHandler.HandleUnknownInput(request.Message, cancellationToken)
        };
        
        await handleTextMessage;
    }
}