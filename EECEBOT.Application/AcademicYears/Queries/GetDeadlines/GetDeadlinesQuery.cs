using EECEBOT.Application.AcademicYears.ResultModels.DeadlinesResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetDeadlines;

public sealed record GetDeadlinesQuery(string Year) : IRequest<ErrorOr<GetDeadlinesQueryResult>>;