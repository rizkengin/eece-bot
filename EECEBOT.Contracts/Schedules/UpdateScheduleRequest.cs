namespace EECEBOT.Contracts.Schedules;

public sealed record UpdateScheduleRequest(
    string ScheduleStartDate,
    IEnumerable<SessionRequest> Sessions);
    
public sealed record SessionRequest(
    string DayOfWeek,
    string Period,
    Guid SubjectId,
    string Lecturer,
    string Location,
    string SessionType,
    string Frequency,
    IEnumerable<string> Sections);