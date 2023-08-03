using EECEBOT.Application.Common.Persistence;
using MediatR;
using Telegram.Bot.Types.Enums;

namespace EECEBOT.Application.TelegramBot.Commands.ChatMemberStatusUpdatedCommand;

internal sealed class ChatMemberStatusUpdatedCommandHandler : IRequestHandler<ChatMemberStatusUpdatedCommand>
{
    private readonly ITelegramUserRepository _telegramUserRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChatMemberStatusUpdatedCommandHandler(
        ITelegramUserRepository telegramUserRepository,
        IUnitOfWork unitOfWork)
    {
        _telegramUserRepository = telegramUserRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChatMemberStatusUpdatedCommand request, CancellationToken cancellationToken)
    {
        if (request.ChatMemberUpdated.NewChatMember.Status == ChatMemberStatus.Kicked)
        {
            _telegramUserRepository.Remove(request.ChatMemberUpdated.From.Id);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}