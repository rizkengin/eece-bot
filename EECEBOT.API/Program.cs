using Azure.Identity;
using EECEBOT.Application;
using EECEBOT.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        var config = builder.AddEnvironmentVariables()
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .Build();
        var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("AzureKeyVaultUrl")!); // Make sure this is set in system environment variables
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
