using EECEBOT.Application.Links.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Links.Queries.GetLinks;

public sealed record GetLinksQuery(string AcademicYear) : IRequest<ErrorOr<GetLinksQueryResult>>;