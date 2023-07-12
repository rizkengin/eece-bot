using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using Microsoft.AspNetCore.Http;
using ErrorOr;

namespace EECEBOT.Application.Common.Persistence;

public interface IAcademicYearRepository
{
    Task<AcademicYear?> GetAcademicYearAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<IEnumerable<Deadline>>> GetDeadlinesAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<IEnumerable<Exam>>> GetExamsAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<IEnumerable<Link>>> GetLinksAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<Schedule?>> GetScheduleAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<LabSchedule?>> GetLabScheduleAsync(Year year, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateScheduleFileAsync(AcademicYear academicYear, IFormFile scheduleFile, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateLabScheduleFileAsync(AcademicYear academicYear, IFormFile labScheduleFile, CancellationToken cancellationToken = default);
}