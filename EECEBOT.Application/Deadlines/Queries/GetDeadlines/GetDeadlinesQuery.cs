using EECEBOT.Application.Deadlines.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Deadlines.Queries.GetDeadlines;

public sealed record GetDeadlinesQuery(string AcademicYear) : IRequest<ErrorOr<GetDeadlinesQueryResult>>;