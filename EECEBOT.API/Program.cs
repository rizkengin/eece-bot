using EECEBOT.API;
using EECEBOT.API.Common;
using EECEBOT.API.Common.Errors;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(workerOptions =>
    {
        workerOptions.UseAspNetCoreIntegration();
        workerOptions.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureAspNetCoreIntegration()
    .ConfigureAppConfiguration(builder =>
    {
        builder.AddAzureVaultConfiguration();
    })
    .ConfigureServices((context, services) =>
    {
        services
            .AddPresentation()
            .AddApplication(context.Configuration)
            .AddInfrastructure(context.Configuration);
    })
    .Build();

ApplicationHealthChecks.Check(host);

host.Run();