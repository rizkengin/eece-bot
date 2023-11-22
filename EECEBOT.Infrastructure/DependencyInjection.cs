using Azure.Identity;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.TelegramUserAggregate;
using EECEBOT.Domain.UserAggregate;
using EECEBOT.Infrastructure.Persistence;
using EECEBOT.Infrastructure.Persistence.Interceptors;
using EECEBOT.Infrastructure.Services;
using EECEBOT.Infrastructure.Services.AcademicYearsResults;
using EECEBOT.Infrastructure.TelegramBot;
using Hangfire;
using Hangfire.PostgreSql;
using Marten;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Core;

namespace EECEBOT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        services
            .AddInterfaces()
            .AddAzureBlobService(configuration)
            .AddPersistence(configuration)
            .AddHangfireServices(configuration);

        return services;
    }
    private static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180);
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UsePostgreSqlStorage(options =>
            {
                options.UseNpgsqlConnection(configuration[configuration["ConnectionStrings:Npgsql"]!]!);
            });
            AddBackgroundJobs();
        });

        services.AddHangfireServer();
    }

    private static void AddBackgroundJobs()
    {
        RecurringJob
            .AddOrUpdate<IBackgroundTasksService>(
                "ProcessOutboxMessages",
                x => x.ProcessOutboxMessagesAsync(),
                "* * * * *");

        RecurringJob
            .AddOrUpdate<IBackgroundTasksService>(
                "RequestGithubRepoStarFromUsers",
                x => x.RequestGithubRepoStarFromUsersAsync(),
                Cron.Monthly(1, 18, 0),
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")
                });

        RecurringJob
            .AddOrUpdate<IBackgroundTasksService>(
                "ExpiredRefreshTokensCleanup",
                x => x.ExpiredRefreshTokensCleanupAsync(),
                Cron.Weekly(DayOfWeek.Sunday));

        RecurringJob
            .AddOrUpdate<IBackgroundTasksService>(
                "AcademicYearResetProcess",
                x => x.AcademicYearResetProcessAsync(),
                Cron.Yearly(9, 1));

        RecurringJob
            .AddOrUpdate<IBackgroundTasksService>(
                "ResultsService",
                x => x.CheckForAcademicYearsResultsAsync(),
                Cron.Minutely);
    }

    private static IServiceCollection AddAzureBlobService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(configuration["AzureBlobStorage"]);
        });

        return services;
    }

    private static IServiceCollection AddInterfaces(this IServiceCollection services)
    {
        services.AddScoped<ITelegramBotMessageHandler, TelegramBotMessageHandler>();
        services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
        services.AddScoped<ITelegramBotCallbackQueryDataHandler, TelegramBotCallbackQueryDataHandler>();
        services.AddScoped<ITimeService, TimeService>();
        services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
        services.AddScoped<IBackgroundTasksService, BackgroundTasksService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
            {
                options.Connection(configuration[configuration["ConnectionStrings:Npgsql"]!]!);

                options.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All,
                    enumStorage: EnumStorage.AsString);

                options.Schema.For<TelegramUser>()
                    .DocumentAlias("Telegram_Users")
                    .Index(x => x.ChatId, x => x.IsUnique = true)
                    .Identity(x => x.Id);

                options.Schema.For<User>()
                    .DocumentAlias("Users")
                    .Index(x => x.Email, x => x.IsUnique = true)
                    .Identity(x => x.Id);

                options.Schema.For<AcademicYear>()
                    .DocumentAlias("Academic_Years")
                    .Identity(x => x.Id);

                options.Schema.For<OutboxMessage>()
                    .DocumentAlias("Outbox_Messages")
                    .Identity(x => x.Id);

                options.Schema.For<AcademicYearResult>()
                    .DocumentAlias("Academic_Years_Results")
                    .Identity(x => x.Id)
                    .Index(x => x.AcademicYear);

                options.Listeners.Add(new ConvertDomainEventsToOutboxMessagesInterceptor());
            }).UseLightweightSessions()
            .ApplyAllDatabaseChangesOnStartup()
            .AssertDatabaseMatchesConfigurationOnStartup();

        return services;
    }

    public static IServiceCollection AddAzureVaultConfiguration(this IServiceCollection services, ConfigurationManager configuration)
    {
        var keyVaultUrl = new Uri(configuration.GetValue<string>("eece_bot_azure_key_vault_url")!);
        var azureCredentials = new DefaultAzureCredential();
        configuration.AddAzureKeyVault(keyVaultUrl, azureCredentials);

        return services;
    }
}