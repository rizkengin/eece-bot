namespace EECEBOT.Contracts.Schedules;

public sealed record GetScheduleSubjectsResponse(IList<SubjectQueryResponse> Subjects);

public sealed record SubjectQueryResponse(
    Guid Id,
    string Name,
    string Code);