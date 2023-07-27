using MediatR;

namespace EECEBOT.Application.TelegramBot.Queries.TelegramQueryExceptionQuery;

public record TelegramQueryExceptionQuery(Exception Exception) : IRequest;