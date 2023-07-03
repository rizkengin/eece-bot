using Azure.Identity;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Exam;
using EECEBOT.Domain.LabSchedule;
using EECEBOT.Domain.Link;
using EECEBOT.Domain.Schedule;
using EECEBOT.Domain.Schedule.Entities;
using EECEBOT.Domain.TelegramUser;
using EECEBOT.Domain.User;
using EECEBOT.Infrastructure.Persistence;
using EECEBOT.Infrastructure.Services;
using EECEBOT.Infrastructure.TelegramBot;
using Marten;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weasel.Postgresql;

namespace EECEBOT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddInterfaces()
            .AddAzureBlobService(configuration)
            .AddPersistence(configuration);

        return services;
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
        services.AddScoped<ILabScheduleRepository, LabScheduleRepository>();
        services.AddScoped<IScheduleRepository, ScheduleRepository>();
        services.AddScoped<ILinksRepository, LinksRepository>();
        services.AddScoped<IExamsRepository, ExamsRepository>();
        services.AddScoped<IDeadlinesRepository, DeadlinesRepository>();
        services.AddScoped<ITimeService,TimeService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration["EECEBOTDb"]!);
            
            options.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All);

            options.Schema.For<TelegramUser>()
                .DocumentAlias("Telegram_Users")
                .Index(x => x.ChatId, x => x.IsUnique = true)
                .Identity(x => x.Id);

            options.Schema.For<User>()
                .DocumentAlias("Users")
                .Index(x => x.Email, x => x.IsUnique = true)
                .Identity(x => x.Id);

            options.Schema.For<Link>()
                .DocumentAlias("Links")
                .Identity(x => x.Id);

            options.Schema.For<Exam>()
                .DocumentAlias("Exams")
                .Identity(x => x.Id);

            options.Schema.For<Schedule>()
                .DocumentAlias("Schedules")
                .Identity(x => x.Id)
                .Index(x => x.AcademicYear, x => x.IsUnique = true);

            options.Schema.For<Session>()
                .DocumentAlias("Sessions")
                .Identity(x => x.Id)
                .ForeignKey<Schedule>(x => x.ScheduleId, dfk => dfk.OnDelete = CascadeAction.Cascade)
                .ForeignKey<Subject>(x => x.SubjectId, dfk => dfk.OnDelete = CascadeAction.Cascade);

            options.Schema.For<Subject>()
                .DocumentAlias("Subjects")
                .Identity(x => x.Id)
                .ForeignKey<Schedule>(x => x.ScheduleId, dfk => dfk.OnDelete = CascadeAction.Cascade);

            options.Schema.For<LabSchedule>()
                .DocumentAlias("Lab_Schedules")
                .Identity(x => x.Id);

        }).UseLightweightSessions()
            .ApplyAllDatabaseChangesOnStartup()
            .AssertDatabaseMatchesConfigurationOnStartup();
    }

    public static void AddAzureVaultConfiguration(this IConfigurationBuilder builder)
    {
        var keyVaultUrl = new Uri(Environment.GetEnvironmentVariable("eece_bot_azure_key_vault_url")!); // Make sure this is set in system environment variables
        var azureCredentials = new DefaultAzureCredential();
        builder.AddAzureKeyVault(keyVaultUrl, azureCredentials);
    }
}