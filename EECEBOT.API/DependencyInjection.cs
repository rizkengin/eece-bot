using EECEBOT.API.Common.Errors;
using EECEBOT.API.Common.Mapping;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Serilog.Events;

namespace EECEBOT.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IHostBuilder host)
    {
        services
            .AddControllers()
            .AddNewtonsoftJson();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddMappings();
        
        AddLogging(host);

        services.AddSingleton<ProblemDetailsFactory, EecebotProblemDetailsFactory>();

        return services;
    }

    private static void AddLogging(IHostBuilder host)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.AzureApp(LogEventLevel.Information)
            .WriteTo.Console()
            .CreateBootstrapLogger();

        host.UseSerilog((context, services, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.AzureApp(LogEventLevel.Information)
            .WriteTo.Console());
    }
}