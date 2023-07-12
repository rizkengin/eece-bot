namespace EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;

public sealed record GetScheduleQueryResult(ScheduleResult Schedule);

public sealed record ScheduleResult(
    Guid Id,
    string ScheduleStartDate,
    string? FileUri,
    IEnumerable<SessionResult> Sessions);
    
public sealed record SessionResult(
SubjectResult Subject,
string DayOfWeek,
string Period,
string Lecturer,
string Location,
string SessionType,
string Frequency,
IEnumerable<string> Sections);

public sealed record SubjectResult(string Name, string Code);