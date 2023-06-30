using System.Reflection;
using EECEBOT.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace EECEBOT.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddTelegramBotServices(configuration)
            .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
    private static IServiceCollection AddTelegramBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("telegram-bot")
            .AddTypedClient<ITelegramBotClient>(httpclient =>
            {
                var botToken = configuration["TelegramBotToken"]!;
                return new TelegramBotClient(botToken, httpclient);
            });

        return services;
    }
}