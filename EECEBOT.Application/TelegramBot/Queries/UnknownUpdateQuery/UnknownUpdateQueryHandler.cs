using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EECEBOT.Application.TelegramBot.Queries.UnknownUpdateQuery;

public sealed class UnknownUpdateQueryHandler : IRequestHandler<UnknownUpdateQuery>
{
    private readonly ILogger<UnknownUpdateQueryHandler> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public UnknownUpdateQueryHandler(ILogger<UnknownUpdateQueryHandler> logger, ITelegramBotClient telegramBotClient)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
    }

    public async Task Handle(UnknownUpdateQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive unknown message type: {MessageType}", request.Update.Message?.Type);

        await _telegramBotClient.SendTextMessageAsync(request.Update.Type switch
        {
            UpdateType.EditedMessage => request.Update.EditedMessage!.Chat.Id,
            UpdateType.CallbackQuery => request.Update.CallbackQuery!.Message!.Chat.Id,
            UpdateType.ChannelPost => request.Update.ChannelPost!.Chat.Id,
            UpdateType.EditedChannelPost => request.Update.EditedChannelPost!.Chat.Id,
            UpdateType.ShippingQuery => request.Update.ShippingQuery!.From.Id,
            UpdateType.PreCheckoutQuery => request.Update.PreCheckoutQuery!.From.Id,
            UpdateType.InlineQuery => request.Update.InlineQuery!.From.Id,
            UpdateType.ChosenInlineResult => request.Update.ChosenInlineResult!.From.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(request), request.Update.Type, null)
        }, "<b>Unknown input! Please try the /help command to view the list of available commands.</b>",
            replyMarkup: new ReplyKeyboardRemove(),
            parseMode: ParseMode.Html,
            cancellationToken: cancellationToken);
    }
}