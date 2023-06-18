using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule;

namespace EECEBOT.Application.Common.Persistence;

public interface ILabScheduleRepository
{
    Task<LabSchedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
}