using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.Common;
using Microsoft.Extensions.Logging;

namespace EECEBOT.Infrastructure.Services;

public class TimeService : ITimeService
{
    private const string AppTimeZoneId = TimeZoneIds.Egypt;
    private const string UtcTimeZoneId = TimeZoneIds.Utc;
    
    private readonly ILogger<TimeService> _logger;

    public TimeService(ILogger<TimeService> logger)
    {
        _logger = logger;
    }

    public DateTimeOffset GetCurrentUtcTime() => DateTimeOffset.UtcNow;

    public DateTime ConvertUtcDateTimeOffsetToAppDateTime(DateTimeOffset utcTime)
        => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(utcTime, AppTimeZoneId)
            .DateTime; 

    public DateTimeOffset ConvertAppDateTimeToUtcDateTimeOffset(DateTime appTimeZoneTime)
        => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(new DateTimeOffset(appTimeZoneTime,
            TimeZoneInfo.FindSystemTimeZoneById(AppTimeZoneId).GetUtcOffset(appTimeZoneTime)),
            UtcTimeZoneId);
    public bool IsAppTimeZoneIdValid()
    {
        var systemTimeZones = TimeZoneInfo.GetSystemTimeZones().ToList();
        
        _logger.LogInformation("The number of timezones found: {Count}", systemTimeZones.Count);
        
        systemTimeZones.ForEach(x => _logger.LogInformation("System time zone: {DisplayName} {Id}", x.DisplayName, x.Id));
        
        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(AppTimeZoneId);
            TimeZoneInfo.FindSystemTimeZoneById(UtcTimeZoneId);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to find system time zone by id");
            return false;
        }
    }

    public string GetAppTimeZoneId() => AppTimeZoneId;
    public string GetUtcTimeZoneId() => UtcTimeZoneId;
}