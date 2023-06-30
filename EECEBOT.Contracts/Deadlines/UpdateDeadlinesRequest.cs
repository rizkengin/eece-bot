namespace EECEBOT.Contracts.Deadlines;

public sealed record UpdateDeadlinesRequest(IEnumerable<DeadlineRequest> Deadlines);

public sealed record DeadlineRequest(string Title,
    string Description,
    string DueDate);