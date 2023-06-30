using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Schedule.Enums;
using EECEBOT.Domain.Schedule.ValueObjects;
using ErrorOr;

namespace EECEBOT.Domain.Schedule;

public class Schedule
{
    private readonly List<Session> _sessions = new();
    
    private Schedule(Guid id,
        AcademicYear academicYear,
        DateOnly scheduleStartDate)
    {
        Id = id;
        AcademicYear = academicYear;
        ScheduleStartDate = scheduleStartDate;
    }
    
    public Guid Id { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    public DateOnly ScheduleStartDate { get; private set; }
    public Uri? FileUri { get; private set; }

    public IReadOnlyCollection<Session> Sessions
    {
        get => _sessions.ToArray();
        private set
        {
            _sessions.Clear();
            _sessions.AddRange(value);
        }
    }
    public static ErrorOr<Schedule> TryCreate(AcademicYear academicYear, DateOnly scheduleStartDate)
    {
        if (scheduleStartDate.DayOfWeek != DayOfWeek.Sunday)
            return Errors.ScheduleErrors.ScheduleStartDateMustBeSunday;

        return new Schedule(Guid.NewGuid(), academicYear, scheduleStartDate);
    }
    
    public void UpdateSessions(IEnumerable<Session> sessions)
    {
       Sessions = sessions.ToList();
    }

    public ErrorOr<Updated> TryUpdateScheduleStartDate(DateOnly scheduleStartDate)
    {
        if (scheduleStartDate.DayOfWeek != DayOfWeek.Sunday)
            return Errors.ScheduleErrors.ScheduleStartDateMustBeSunday;
        
        ScheduleStartDate = scheduleStartDate;
        
        return new Updated();
    }
    
    public void UpdateFileUri(Uri fileUri) => FileUri = fileUri;
    public WeekType GetWeekType(DateOnly date)
    {
        var daysSpan = date.DayNumber - ScheduleStartDate.DayNumber;
        
        return daysSpan % 2 == 0 ? WeekType.Even : WeekType.Odd;
    }
}