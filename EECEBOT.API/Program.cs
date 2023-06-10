using Azure.Identity;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("eece_bot_azure_key_vault_url")!); // Make sure this is set in system environment variables
        var azureCredentials = new DefaultAzureCredential();
        builder.AddAzureKeyVault(keyVaultUrl, azureCredentials);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddApplication(context.Configuration)
            .AddInfrastructure(context.Configuration);
    })
    .Build();

host.Run();
