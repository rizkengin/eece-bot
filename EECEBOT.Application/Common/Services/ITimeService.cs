namespace EECEBOT.Application.Common.Services;

public interface ITimeService
{
    DateTimeOffset GetCurrentUtcTime();
    DateTime ConvertUtcDateTimeOffsetToAppDateTime(DateTimeOffset utcTime);
    DateTimeOffset ConvertAppDateTimeToUtcDateTimeOffset(DateTime appTimeZoneTime);
    bool IsAppTimeZoneIdValid();
    string GetAppTimeZoneId();
    string GetUtcTimeZoneId();
}