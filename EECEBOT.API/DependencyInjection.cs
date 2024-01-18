using EECEBOT.API.Common.Errors;
using EECEBOT.API.Common.Mapping;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Serilog;
using Serilog.Events;

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

        return services;
    }

    private static void AddLogging(
        IHostBuilder host, 
        ConfigurationManager configuration, 
        IWebHostEnvironment environment)
    {
        var seqUrl = environment.IsDevelopment()
            ? configuration.GetValue<string>("Seq:Url")
            ?? throw new InvalidOperationException("Seq URL is missing")
            : configuration["Seq-Container-URL"]
            ?? throw new InvalidOperationException("Seq URL is missing");
        
        var seqApiKey = environment.IsDevelopment()
            ? configuration.GetValue<string>("Seq:ApiKey") 
            ?? throw new InvalidOperationException("Seq API key is missing")
            : configuration[configuration.GetValue<string>("Seq:ApiKeyKey") 
            ?? throw new InvalidOperationException("Seq Api key key is missing")];

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Seq(seqUrl, apiKey: seqApiKey)
            .WriteTo.Console()
            .CreateBootstrapLogger();

        host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Seq(seqUrl, apiKey: seqApiKey)
            .WriteTo.Console());
    }
}