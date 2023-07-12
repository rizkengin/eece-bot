namespace EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;

public sealed record GetScheduleSubjectsQueryResult(IEnumerable<SubjectQueryResult> Subjects);

public sealed record SubjectQueryResult(
    Guid Id,
    string Name,
    string Code);