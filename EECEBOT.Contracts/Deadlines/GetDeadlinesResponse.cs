namespace EECEBOT.Contracts.Deadlines;

public sealed record GetDeadlinesResponse(IEnumerable<DeadlineResponse> Deadlines);

public  record DeadlineResponse(Guid Id,
    string Title,
    string Description,
    string DueDate);