using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.Enums;
using EECEBOT.Domain.TelegramUser;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.Enums;

namespace EECEBOT.Application.TelegramBot.Queries.MessageQuery;

internal sealed class MessageReceivedQueryHandler : IRequestHandler<MessageReceivedQuery>
{
    private readonly ILogger<MessageReceivedQueryHandler> _logger;
    private readonly ITelegramBotMessageHandler _telegramBotMessageHandler;
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MessageReceivedQueryHandler(ILogger<MessageReceivedQueryHandler> logger,
        ITelegramBotMessageHandler telegramBotMessageHandler,
        ITelegramUserRepository telegramUserRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _telegramBotMessageHandler = telegramBotMessageHandler;
        _telegramUserRepository = telegramUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(MessageReceivedQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", request.Message.Type);

        var user = await _telegramUserRepository.GetByChatIdAsync(request.Message.Chat.Id, cancellationToken);
        
        if (user is null)
        {
            user = TelegramUser.Create(
                request.Message.From!.FirstName,
                request.Message.From.Id,
                request.Message.Chat.Id,
                request.Message.From.LastName,
                request.Message.From.Username
            );
            
            _telegramUserRepository.Create(user);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _telegramBotMessageHandler.HandleStartFlow(request.Message, cancellationToken);
            
            return;
        }
        
        if (request.Message.Type != MessageType.Text)
        {
            await _telegramBotMessageHandler.HandleUnknownInput(request.Message, cancellationToken);
            return;
        }

        switch (user.CurrentState)
        {
            case CurrentState.PickingAcademicYear:
                switch(request.Message.Text) 
                {
                    case "1st year":
                        _telegramUserRepository.UpdateAcademicYear(user, AcademicYear.FirstYear);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "2nd year":
                        _telegramUserRepository.UpdateAcademicYear(user, AcademicYear.SecondYear);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "3rd year":
                        _telegramUserRepository.UpdateAcademicYear(user, AcademicYear.ThirdYear);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "4th year":
                        _telegramUserRepository.UpdateAcademicYear(user, AcademicYear.FourthYear);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    default:
                        await _telegramBotMessageHandler.HandlePickingAcademicYearError(request.Message, cancellationToken);
                        return;
                }
            
                await _telegramBotMessageHandler.HandlePickingSectionFlow(request.Message, cancellationToken);

                return;
            
            case CurrentState.PickingSection:
                switch (request.Message.Text)
                {
                    case "Section 1":
                        _telegramUserRepository.UpdateSection(user, Section.SectionOne);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "Section 2":
                        _telegramUserRepository.UpdateSection(user, Section.SectionTwo);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "Section 3":
                        _telegramUserRepository.UpdateSection(user, Section.SectionThree);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    case "Section 4":
                        _telegramUserRepository.UpdateSection(user, Section.SectionFour);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);
                        break;
                    default:
                        await _telegramBotMessageHandler.HandlePickingSectionFlowError(request.Message, cancellationToken);
                        return;
                }
            
                await _telegramBotMessageHandler.HandlePickingBenchNumberFlow(request.Message, cancellationToken);
            
                return;
            
            case CurrentState.PickingBenchNumber:
                if (!int.TryParse(request.Message.Text, out var benchNumber))
                {
                    await _telegramBotMessageHandler.HandlePickingBenchNumberFlowError(request.Message, cancellationToken);
                    return;
                }
            
                _telegramUserRepository.UpdateBenchNumber(user, benchNumber);
            
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            
                await _telegramBotMessageHandler.HandleHelpCommand(user, request.Message, cancellationToken);
            
                return;
            
            case CurrentState.None:
                
                var handleTextMessage = request.Message.Text switch
                {
                    "/schedule" => _telegramBotMessageHandler.HandleScheduleCommand(user, request.Message, cancellationToken),
                    "/exams" => _telegramBotMessageHandler.HandleExamsCommand(user, request.Message, cancellationToken),
                    "/deadlines" => _telegramBotMessageHandler.HandleDeadlinesCommand(user, request.Message, cancellationToken),
                    "/links" => _telegramBotMessageHandler.HandleLinksCommand(user, request.Message, cancellationToken),
                    "/help" => _telegramBotMessageHandler.HandleHelpCommand(user, request.Message, cancellationToken),
                    "/reset" => _telegramBotMessageHandler.HandleResetCommand(user, request.Message, cancellationToken),
                    _ => _telegramBotMessageHandler.HandleUnknownInput(request.Message, cancellationToken)
                };
        
                await handleTextMessage;
                
                return;
            
            default:
                throw new ArgumentOutOfRangeException(user.CurrentState.ToString());
        }
    }
}