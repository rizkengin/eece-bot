namespace EECEBOT.Contracts.Exams;

public sealed record UpdateExamsRequest(List<ExamRequest> Exams);

public sealed record ExamRequest(string Name,
    string ExamType,
    string Description,
    string? Location,
    string Date);