using EECEBOT.API.Common.Errors.Exceptions;
using EECEBOT.Application.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EECEBOT.API.Common;

public static class ApplicationHealthChecks
{
    public static void Check(IHost host)
    {
        CheckTimeService(host.Services);
    }

    private static void CheckTimeService(IServiceProvider serviceProvider)
    {
        var timeService = serviceProvider.GetRequiredService<ITimeService>();
        
        if (!timeService.IsAppTimeZoneIdValid())
        {
            throw new InvalidApplicationTimeZoneIdException(timeService.GetAppTimeZoneId());
        }
    }
}