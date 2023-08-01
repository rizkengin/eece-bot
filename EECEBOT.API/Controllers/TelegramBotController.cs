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

    public TelegramBotController(ISender sender)
    {
        _sender = sender;
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
                _                                              => _sender.Send(new UnknownUpdateQuery(update))
            };

            await handler;
        }
        catch (Exception e)
        {
            try
            {
                await _sender.Send(new TelegramQueryExceptionQuery(e));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}