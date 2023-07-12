namespace EECEBOT.Application.AcademicYears.ResultModels.DeadlinesResultModels;

public record GetDeadlinesQueryResult(IEnumerable<DeadlineResult> Deadlines);

public record DeadlineResult(Guid Id,
    string Title,
    string Description,
    string DueDate);