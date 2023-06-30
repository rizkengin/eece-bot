namespace EECEBOT.Contracts.Exams;

public record GetExamsResponse(IEnumerable<ExamResponse> Exams);

public record ExamResponse(string Name,
    string ExamType,
    string Description,
    string? Location,
    string DateTime);