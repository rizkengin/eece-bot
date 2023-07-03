namespace EECEBOT.Application.Schedules.ResultModels;

public sealed record GetScheduleSubjectsQueryResult(IEnumerable<SubjectQueryResult> Subjects);

public sealed record SubjectQueryResult(
    Guid Id,
    string Name,
    string Code);