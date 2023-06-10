using MediatR;
using Telegram.Bot.Types;

namespace EECEBOT.Application.TelegramBot.Queries.MessageQuery;

public sealed record MessageReceivedQuery(Message Message) : IRequest;
