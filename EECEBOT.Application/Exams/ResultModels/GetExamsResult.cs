namespace EECEBOT.Application.Exams.ResultModels;

public record GetExamsResult(IEnumerable<ExamResult> Exams);

public record ExamResult(string Name,
    string ExamType,
    string Description,
    string? Location,
    string DateTime);