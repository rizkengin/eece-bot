using EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLinks;

public sealed record UpdateLinksCommand(
    List<UpdateLinkRequest> Links,
    string Year) : IRequest<ErrorOr<UpdateLinksResult>>;

public sealed record UpdateLinkRequest(
    string Name,
    string Url);