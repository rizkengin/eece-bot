using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule;

namespace EECEBOT.Application.Common.Persistence;

public interface IScheduleRepository
{
    Task <Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
}