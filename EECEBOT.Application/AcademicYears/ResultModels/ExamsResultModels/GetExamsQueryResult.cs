namespace EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;

public record GetExamsQueryResult(IEnumerable<ExamResult> Exams);

public record ExamResult(string Name,
    string ExamType,
    string Description,
    string? Location,
    string DateTime);