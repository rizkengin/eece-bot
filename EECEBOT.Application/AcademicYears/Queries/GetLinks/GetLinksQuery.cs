using EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetLinks;

public sealed record GetLinksQuery(string Year) : IRequest<ErrorOr<GetLinksQueryResult>>;