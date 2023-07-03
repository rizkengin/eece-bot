namespace EECEBOT.Contracts.LabSchedules;

public sealed record GetLabScheduleResponse(LabScheduleResponse LabSchedule);

public sealed record LabScheduleResponse(
    Guid Id,
    string SplitMethod,
    string? FileUri,
    IEnumerable<LabResponse> Labs);
    
public sealed record LabResponse(
    string Name,
    string Date,
    string Location,
    string Section,
    int BenchNumbersRangeStart,
    int BenchNumbersRangeEnd);