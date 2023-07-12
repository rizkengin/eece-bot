using EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetLinks;

internal sealed class GetLinksQueryHandler : IRequestHandler<GetLinksQuery, ErrorOr<GetLinksQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;

    public GetLinksQueryHandler(IAcademicYearRepository academicYearRepository)
    {
        _academicYearRepository = academicYearRepository;
    }

    public async Task<ErrorOr<GetLinksQueryResult>> Handle(GetLinksQuery request, CancellationToken cancellationToken)
    {
        var linksResult = await _academicYearRepository
            .GetLinksAsync(Enum.Parse<Year>(request.Year, ignoreCase:true),
                cancellationToken);
        
        if (linksResult.IsError)
            return linksResult.Errors;
        
        return new GetLinksQueryResult(linksResult.Value.Select(l => new LinkResult(l.Id, l.Name, l.Url.ToString())));
    }
}