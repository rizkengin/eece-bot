using EECEBOT.API.Common.Errors;
using EECEBOT.API.Common.Mapping;
using EECEBOT.API.StartupTasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Datadog.Logs;

namespace EECEBOT.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(
        this IServiceCollection services, 
        IHostBuilder host, 
        ConfigurationManager configuration, 
        IWebHostEnvironment environment)
    {
        services
            .AddControllers()
            .AddNewtonsoftJson();

        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddMappings();
        
        AddLogging(host, configuration, environment);

        services.AddSingleton<ProblemDetailsFactory, EecebotProblemDetailsFactory>();

        services.AddHostedService<ApplicationStartupCheck>();

        return services;
    }

    private static void AddLogging(
        IHostBuilder host, 
        ConfigurationManager configuration, 
        IWebHostEnvironment environment)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Npgsql.Command", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        host.UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Npgsql.Command", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            if (!environment.IsDevelopment())
            {
                var datadogConfiguration = new DatadogConfiguration(url: configuration["Datadog:Url"]);
                
                loggerConfiguration.WriteTo.DatadogLogs(
                    apiKey: configuration["Datadog:ApiKey"],
                    service: configuration["Datadog:ServiceName"],
                    configuration: datadogConfiguration);
            }
        });
    }
}