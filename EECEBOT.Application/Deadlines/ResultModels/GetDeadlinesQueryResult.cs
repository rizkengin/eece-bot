namespace EECEBOT.Application.Deadlines.ResultModels;

public record GetDeadlinesQueryResult(IEnumerable<DeadlineResult> Deadlines);

public record DeadlineResult(Guid Id,
    string Title,
    string Description,
    string DueDate);