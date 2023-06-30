namespace EECEBOT.Contracts.Schedules;

public sealed record UpdateScheduleRequest(
    string ScheduleStartDate,
    IEnumerable<SessionRequest> Sessions);
    
public sealed record SessionRequest(
    string DayOfWeek,
    string Period,
    SubjectRequest Subject,
    string Lecturer,
    string Location,
    string SessionType,
    string Frequency,
    IEnumerable<string> Sections);
    
public sealed record SubjectRequest(string Name,
    string Code);