using EECEBOT.Application.Deadlines.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Deadlines.Commands;

public sealed record UpdateDeadlinesCommand(List<UpdateDeadlineRequest> Deadlines, string AcademicYear) : IRequest<ErrorOr<UpdateDeadlinesResult>>;

public sealed record UpdateDeadlineRequest(string Title,
    string Description,
    string DueDate);