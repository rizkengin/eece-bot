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

    public async Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.Id == scheduleId, token: cancellationToken);
    }

    public async Task<Schedule?> GetByIdAndAcademicYearAsync(Guid scheduleId, AcademicYear academicYear,
        CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.Id == scheduleId && x.AcademicYear == academicYear, token: cancellationToken);
    }

    public async Task<Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.AcademicYear == academicYear, token: cancellationToken);
    }

    public void AddSchedule(Schedule schedule)
    {
        _documentSession.Insert(schedule);
    }

    public async Task UpdateScheduleAsync(Schedule schedule, AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        var existingSchedule = await GetByAcademicYearAsync(academicYear, cancellationToken);

        if (existingSchedule is not null)
            _documentSession.Delete(existingSchedule);
        
        _documentSession.Insert(schedule);
    }
}