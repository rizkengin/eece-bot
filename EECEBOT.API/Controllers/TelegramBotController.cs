using EECEBOT.Application.TelegramBot.Commands.ChatMemberStatusUpdatedCommand;
using EECEBOT.Application.TelegramBot.Queries.CallbackQueryQuery;
using EECEBOT.Application.TelegramBot.Queries.MessageQuery;
using EECEBOT.Application.TelegramBot.Queries.TelegramQueryExceptionQuery;
using EECEBOT.Application.TelegramBot.Queries.UnknownUpdateQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace EECEBOT.API.Controllers;

[ApiController]
[Route("telegram-bot")]
public class TelegramBotController : ControllerBase
{
    private readonly ISender _sender;
    private readonly ILogger<TelegramBotController> _logger;

    public TelegramBotController(
        ISender sender,
        ILogger<TelegramBotController> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    [HttpPost("webhook")]
    public async Task Webhook([FromBody] Update update)
    {
        try
        {
            var handler = update switch
            {
                { Message: { } message }                       => _sender.Send(new MessageReceivedQuery(message)),
                { CallbackQuery: { } callbackQuery }           => _sender.Send(new CallbackQueryQuery(callbackQuery)),
                { MyChatMember: { } chatMemberUpdated }        => _sender.Send(new ChatMemberStatusUpdatedCommand(chatMemberUpdated)),
                _                                              => _sender.Send(new UnknownUpdateQuery(update))
            };

            await handler;
        }
        catch (Exception e)
        {
            try
            {
                _logger.LogError(e, "Exception happened while handling Telegram webhook update");
                
                await _sender.Send(new TelegramQueryExceptionQuery(e));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception happened while handling Telegram webhook update");
            }
        }
    }
}