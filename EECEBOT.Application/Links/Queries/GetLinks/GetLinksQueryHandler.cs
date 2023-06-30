using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Links.ResultModels;
using EECEBOT.Domain.Common.Enums;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Links.Queries.GetLinks;

internal sealed class GetLinksQueryHandler : IRequestHandler<GetLinksQuery, ErrorOr<GetLinksQueryResult>>
{
    private readonly ILinksRepository _linksRepository;

    public GetLinksQueryHandler(ILinksRepository linksRepository)
    {
        _linksRepository = linksRepository;
    }

    public async Task<ErrorOr<GetLinksQueryResult>> Handle(GetLinksQuery request, CancellationToken cancellationToken)
    {
        var links = await _linksRepository
            .GetLinksByAcademicYearAsync(Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase:true),
                cancellationToken);
        
        return new GetLinksQueryResult(links.Select(l => new LinkResult(l.Id, l.Name, l.Url.ToString())));
    }
}