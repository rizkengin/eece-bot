using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Exam;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class ExamsRepository : IExamsRepository
{
    private readonly IDocumentSession _session;

    public ExamsRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<IEnumerable<Exam>> GetExamsAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _session.Query<Exam>()
            .Where(e => e.AcademicYear == academicYear)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateExamsAsync(IEnumerable<Exam> exams, AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        var existingExams = await GetExamsAsync(academicYear, cancellationToken);

        foreach (var exam in existingExams)
            _session.Delete(exam);

        foreach (var exam in exams)
            _session.Insert(exam);
    }
}