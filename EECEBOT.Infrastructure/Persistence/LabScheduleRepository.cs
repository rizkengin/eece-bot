using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class LabScheduleRepository : ILabScheduleRepository
{
    private readonly IDocumentSession _documentSession;

    public LabScheduleRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<LabSchedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<LabSchedule>()
            .FirstOrDefaultAsync(x => x.AcademicYear == academicYear, token: cancellationToken);
    }
}