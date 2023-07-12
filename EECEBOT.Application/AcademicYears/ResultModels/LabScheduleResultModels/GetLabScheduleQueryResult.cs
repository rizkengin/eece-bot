namespace EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;

public sealed record GetLabScheduleQueryResult(LabScheduleResult LabSchedule);

public sealed record LabScheduleResult(
    Guid Id,
    string SplitMethod,
    string? FileUri,
    IEnumerable<LabResult> Labs);
    
public sealed record LabResult(
    string Name,
    string Date,
    string Location,
    string Section,
    int BenchNumbersRangeStart,
    int BenchNumbersRangeEnd);
