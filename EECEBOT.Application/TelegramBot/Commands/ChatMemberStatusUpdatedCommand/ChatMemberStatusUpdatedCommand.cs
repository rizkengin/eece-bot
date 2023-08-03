using MediatR;
using Telegram.Bot.Types;

namespace EECEBOT.Application.TelegramBot.Commands.ChatMemberStatusUpdatedCommand;

public record ChatMemberStatusUpdatedCommand(ChatMemberUpdated ChatMemberUpdated) : IRequest;