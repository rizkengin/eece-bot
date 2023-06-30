using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Link;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class LinksRepository : ILinksRepository
{
    private readonly IDocumentSession _documentSession;
    
    public LinksRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<IEnumerable<Link>> GetLinksByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Link>()
            .Where(l => l.AcademicYear == academicYear)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateLinksAsync(IEnumerable<Link> links, AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        var existingLinks = await GetLinksByAcademicYearAsync(academicYear, cancellationToken);

        foreach (var link in existingLinks)
        {
            _documentSession.Delete(link);
        }

        foreach (var link in links)
        {
            _documentSession.Insert(link);
        }
    }
}