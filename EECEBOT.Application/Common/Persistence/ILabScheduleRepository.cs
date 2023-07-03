using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule;
using EECEBOT.Domain.LabSchedule.ValueObjects;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Application.Common.Persistence;

public interface ILabScheduleRepository
{
    Task<LabSchedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task<LabSchedule?> GetByIdAndAcademicYearAsync(Guid labScheduleId, AcademicYear academicYear, CancellationToken cancellationToken = default);
    void UpdateLabScheduleSessionsAsync(LabSchedule labSchedule, IEnumerable<Lab> labs);
    Task<ErrorOr<Updated>> UpdateLabScheduleFileAsync(LabSchedule labSchedule, IFormFile labScheduleFile, CancellationToken cancellationToken = default);
    void Add(LabSchedule labSchedule);
}