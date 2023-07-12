using EECEBOT.Application.AcademicYears.ResultModels.DeadlinesResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateDeadlines;

public sealed record UpdateDeadlinesCommand(List<UpdateDeadlineRequest> Deadlines, string Year) : IRequest<ErrorOr<UpdateDeadlinesResult>>;

public sealed record UpdateDeadlineRequest(string Title,
    string Description,
    string DueDate);