namespace EECEBOT.Application.Common.Services;

public interface ITimeService
{
    DateTime GetCurrentUtcTime();
    DateTime? ConvertUtcToTimeZoneTime(DateTime utcTime, string timeZoneId);
}