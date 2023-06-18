using EECEBOT.Application.Common.Services;

namespace EECEBOT.Infrastructure.Services;

public class TimeService : ITimeService
{
    public DateTime GetCurrentUtcTime() => DateTime.UtcNow;

    public DateTime? ConvertUtcToTimeZoneTime(DateTime utcTime, string timeZoneId)
    {
        try
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }
}