using System.Reflection;
using System.Security.Claims;
using System.Text;
using EECEBOT.Application.Authentication;
using EECEBOT.Application.Common.AuthenticationServices.IdentityService;
using EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;
using EECEBOT.Application.Common.AuthenticationServices.PasswordHasher;
using EECEBOT.Application.Common.Behaviors;
using EECEBOT.Application.Http;
using EECEBOT.Domain.Common.Enums;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Telegram.Bot;

namespace EECEBOT.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services
            .AddTelegramBotServices(configuration, isDevelopment)
            .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddAuth(configuration);
        
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
    
    private static void AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = new JwtSettings
        {
            Secret = configuration["APISecret"]!,
        };
        configuration.Bind(JwtSettings.SectionName, jwtSettings);
        services.AddSingleton(Options.Create(jwtSettings));

        services.AddScoped<IJwtTokenProvider, JwtTokenProvider>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IIdentityService, IdentityService>();

        services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtSettings.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AcademicYearRepresentatives, policy =>
            {
                policy.RequireAuthenticatedUser();
                
                policy.RequireRole(Role.FirstYearRepresentative.ToString(),
                    Role.SecondYearRepresentative.ToString(),
                    Role.ThirdYearRepresentative.ToString(),
                    Role.FourthYearRepresentative.ToString());

                policy.RequireAssertion(context =>
                {
                    var isRoleClaimPresent = Enum.TryParse<Role>(context.User.FindFirstValue(ClaimTypes.Role)!, true, out var role);
                    
                    if (!isRoleClaimPresent)
                    {
                        return false;
                    }
                
                    if (context.Resource is HttpContext httpContext &&
                        httpContext.Request.RouteValues.TryGetValue("year", out var yearValue))
                    {
                        return role switch
                        {
                            Role.FirstYearRepresentative => string.Equals((string)yearValue!, HttpRouteKeys.FirstYear, StringComparison.OrdinalIgnoreCase),
                            Role.SecondYearRepresentative => string.Equals((string)yearValue!, HttpRouteKeys.SecondYear, StringComparison.OrdinalIgnoreCase),
                            Role.ThirdYearRepresentative => string.Equals((string)yearValue!, HttpRouteKeys.ThirdYear, StringComparison.OrdinalIgnoreCase),
                            Role.FourthYearRepresentative => string.Equals((string)yearValue!, HttpRouteKeys.FourthYear, StringComparison.OrdinalIgnoreCase),
                            _ => false
                        };
                    }
                    
                    return false;
                });
            });
        });
    }
    
    private static IServiceCollection AddTelegramBotServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddHttpClient("telegram-bot")
            .AddTypedClient<ITelegramBotClient>(httpclient =>
            {
                var botToken = isDevelopment ? configuration["TelegramDevelopmentBotToken"]! : configuration["TelegramProductionBotToken"]!;
                return new TelegramBotClient(botToken, httpclient);
            });

        return services;
    }
}