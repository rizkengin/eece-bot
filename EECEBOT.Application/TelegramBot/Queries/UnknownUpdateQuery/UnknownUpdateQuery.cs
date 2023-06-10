using MediatR;
using Telegram.Bot.Types;

namespace EECEBOT.Application.TelegramBot.Queries.UnknownUpdateQuery;

public sealed record UnknownUpdateQuery(Update Update) : IRequest;