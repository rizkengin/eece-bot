using EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLinks;

public sealed record UpdateLinksCommand(List<(string name,string url)> LinksTuples, string Year) : IRequest<ErrorOr<UpdateLinksResult>>;