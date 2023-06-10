using EECEBOT.Application.TelegramBot.Queries.CallbackQueryQuery;
using EECEBOT.Application.TelegramBot.Queries.MessageQuery;
using EECEBOT.Application.TelegramBot.Queries.UnknownUpdateQuery;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace EECEBOT.API.Functions;

public class TelegramBot
{
    private readonly ILogger<TelegramBot> _logger;
    private readonly ISender _sender;

    public TelegramBot(ILoggerFactory loggerFactory, ISender sender)
    {
        _sender = sender;
        _logger = loggerFactory.CreateLogger<TelegramBot>();
    }

    [Function("TelegramBotWebhook")]
    public async Task TelegramWebhook([HttpTrigger(AuthorizationLevel.Anonymous,
        "post",
        Route = "bot-webhook")]
        HttpRequestData request)
    {
        _logger.LogInformation("Received a Telegram Bot Webhook request");
        
        try
        {
            var body = await request.ReadAsStringAsync();
            
            var update = JsonConvert.DeserializeObject<Update>(body!);
            
            if (update is null)
            {
                _logger.LogWarning("Unable to deserialize the request body");
                return;
            }
            
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
            _logger.LogError("Exception : {Message}", e.Message);
        }
    }
}
