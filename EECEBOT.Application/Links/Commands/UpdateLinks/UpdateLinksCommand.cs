using EECEBOT.Application.Links.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Links.Commands.UpdateLinks;

public sealed record UpdateLinksCommand(List<(string name,string url)> LinksTuples, string AcademicYear) : IRequest<ErrorOr<UpdateLinksResult>>;