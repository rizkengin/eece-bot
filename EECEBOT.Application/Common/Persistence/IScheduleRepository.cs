using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule;

namespace EECEBOT.Application.Common.Persistence;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Schedule?> GetByIdAndAcademicYearAsync(Guid scheduleId, AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task<Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    void AddSchedule(Schedule schedule);
    Task UpdateScheduleAsync(Schedule schedule, AcademicYear academicYear, CancellationToken cancellationToken = default);
}