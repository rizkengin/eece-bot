using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Common.TelegramBot;
using EECEBOT.Domain.Exam;
using EECEBOT.Domain.LabSchedule;
using EECEBOT.Domain.Link;
using EECEBOT.Domain.Schedule;
using EECEBOT.Domain.TelegramUser;
using EECEBOT.Domain.User;
using EECEBOT.Infrastructure.Persistence;
using EECEBOT.Infrastructure.Services;
using EECEBOT.Infrastructure.TelegramBot;
using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EECEBOT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
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
        services.AddPersistence(configuration);
        
        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration["EECEBOTDb"]!);
            
            options.UseDefaultSerialization(nonPublicMembersStorage: NonPublicMembersStorage.All);

            options.Schema.For<TelegramUser>()
                .Index(x => x.ChatId, x => x.IsUnique = true)
                .Identity(x => x.Id);

            options.Schema.For<User>()
                .Index(x => x.Email, x => x.IsUnique = true)
                .Identity(x => x.Id);

            options.Schema.For<Link>()
                .Identity(x => x.Id);

            options.Schema.For<Exam>()
                .Identity(x => x.Id);

            options.Schema.For<Schedule>()
                .Identity(x => x.Id)
                .Index(x => x.AcademicYear, x => x.IsUnique = true);

            options.Schema.For<LabSchedule>()
                .Identity(x => x.Id);

        }).UseLightweightSessions()
            .ApplyAllDatabaseChangesOnStartup()
            .AssertDatabaseMatchesConfigurationOnStartup();
    }
}