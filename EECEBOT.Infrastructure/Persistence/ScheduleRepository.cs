using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IDocumentSession _documentSession;

    public ScheduleRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.AcademicYear == academicYear, token: cancellationToken);
    }
}