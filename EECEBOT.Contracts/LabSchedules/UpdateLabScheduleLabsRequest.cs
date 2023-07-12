namespace EECEBOT.Contracts.LabSchedules;

public sealed record UpdateLabScheduleLabsRequest(IEnumerable<LabRequest> Labs);

public sealed record LabRequest(
    string Name,
    string Date,
    string Location,
    string Section,
    int BenchNumbersRangeStart,
    int BenchNumbersRangeEnd);