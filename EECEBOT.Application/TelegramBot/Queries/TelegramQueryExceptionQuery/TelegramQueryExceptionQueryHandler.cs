using MediatR;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace EECEBOT.Application.TelegramBot.Queries.TelegramQueryExceptionQuery;

internal sealed class TelegramQueryExceptionQueryHandler : IRequestHandler<TelegramQueryExceptionQuery>
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _exceptionChatId;

    public TelegramQueryExceptionQueryHandler(ITelegramBotClient botClient, IConfiguration configuration)
    {
        _botClient = botClient;
        _exceptionChatId = configuration["ExceptionChatId"] ?? throw new ArgumentNullException(nameof(configuration),"ExceptionChatId is null");
    }

    public async Task Handle(TelegramQueryExceptionQuery request, CancellationToken cancellationToken)
    {
        await _botClient.SendTextMessageAsync(
            _exceptionChatId,
            $"Exception has been thrown: {request.Exception.Message} \n\n {request.Exception.StackTrace}",
            cancellationToken: cancellationToken);
    }
}