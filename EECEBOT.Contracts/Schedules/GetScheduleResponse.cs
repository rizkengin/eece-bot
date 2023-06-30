namespace EECEBOT.Contracts.Schedules;

public sealed record GetScheduleResponse(ScheduleResponse Schedule);

public sealed record ScheduleResponse(
    Guid Id,
    string ScheduleStartDate,
    string? FileUri,
    IEnumerable<SessionResponse> Sessions);
    
public sealed record SessionResponse(
    SubjectResponse Subject,
    string DayOfWeek,
    string Period,
    string Lecturer,
    string Location,
    string SessionType,
    string Frequency,
    IEnumerable<string> Sections);

public sealed record SubjectResponse(string Name, string Code);