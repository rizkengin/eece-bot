using Azure.Identity;
using EECEBOT.API;
using EECEBOT.API.Common;
using EECEBOT.API.Common.Errors;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
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
        var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("eece_bot_azure_key_vault_url")!); // Make sure this is set in system environment variables
        var azureCredentials = new DefaultAzureCredential();
        builder.AddAzureKeyVault(keyVaultUrl, azureCredentials);
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