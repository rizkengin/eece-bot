using EECEBOT.API.Common.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace EECEBOT.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddMappings()
            .AddLogging();
        
        return services;
    }
}