using EECEBOT.API.Common.Exceptions;
using EECEBOT.Application.Common.AuthenticationServices.PasswordHasher;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.UserAggregate;
using Marten;

namespace EECEBOT.API.StartupTasks;

internal sealed class ApplicationStartupCheck : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public ApplicationStartupCheck(
        IServiceProvider serviceProvider, 
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        CheckTimeService(_serviceProvider);
        SeedAcademicYears(_serviceProvider);
        SeedAcademicYearRepresentatives(_serviceProvider, _configuration);
    }
    
    private static void CheckTimeService(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        var timeService = scope.ServiceProvider.GetRequiredService<ITimeService>();
        
        if (!timeService.IsAppTimeZoneIdValid())
        {
            throw new InvalidApplicationTimeZoneIdException(timeService.GetAppTimeZoneId());
        }
    }
    
    private static void SeedAcademicYears(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        
        var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var existingAcademicYears = documentSession.Query<AcademicYear>().ToList();

        if (existingAcademicYears.Any()) return;

        AcademicYear[] academicYears =
        {
            AcademicYear.Create(Year.FirstYear),
            AcademicYear.Create(Year.SecondYear),
            AcademicYear.Create(Year.ThirdYear),
            AcademicYear.Create(Year.FourthYear)
        };

        documentSession.Store(academicYears);

        documentSession.SaveChanges();
    }

    private static void SeedAcademicYearRepresentatives(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();

        var documentSession = scope.ServiceProvider.GetRequiredService<IDocumentSession>();

        var existingAcademicYearRepresentatives = documentSession
            .Query<User>()
            .Where(u => u.Role != Role.SuperAdmin)
            .ToList();

        if (existingAcademicYearRepresentatives.Count != 0) return;

        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        User[] academicYearRepresentatives =
        {
            User.Create(
                "First Year",
                "Representative",
                "first_year@eecebot.com",
                "00000000000",
                passwordHasher.HashPassword(configuration["first-year-account-password"]!),
                Role.FirstYearRepresentative),
            
            User.Create(
                "Second Year",
                "Representative",
                "second_year@eecebot.com",
                "00000000000",
                passwordHasher.HashPassword(configuration["second-year-account-password"]!),
                Role.SecondYearRepresentative),
            
            User.Create(
                "Third Year",
                "Representative",
                "third_year@eecebot.com",
                "00000000000",
                passwordHasher.HashPassword(configuration["third-year-account-password"]!),
                Role.ThirdYearRepresentative),
            
            User.Create(
                "Fourth Year",
                "Representative",
                "fourth_year@eecebot.com",
                "00000000000",
                passwordHasher.HashPassword(configuration["fourth-year-account-password"]!),
                Role.FourthYearRepresentative)
        };
        
        documentSession.Store(academicYearRepresentatives);
        
        documentSession.SaveChanges();
    }
}