using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Deadline;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class DeadlinesRepository : IDeadlinesRepository
{
    private readonly IDocumentSession _session;

    public DeadlinesRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<Deadline>> GetDeadlinesAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _session.Query<Deadline>()
            .Where(a => a.AcademicYear == academicYear)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateDeadlinesAsync(IEnumerable<Deadline> deadlines, AcademicYear academicYear,
        CancellationToken cancellationToken = default)
    {
        var existingDeadlines = await GetDeadlinesAsync(academicYear, cancellationToken);

        foreach (var deadline in existingDeadlines)
            _session.Delete(deadline);
        
        foreach (var deadline in deadlines)
            _session.Insert(deadline);
    }
}