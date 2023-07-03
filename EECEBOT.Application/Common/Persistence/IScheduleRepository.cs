using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule;
using EECEBOT.Domain.Schedule.Entities;
using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Application.Common.Persistence;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default);
    Task<Schedule?> GetByIdAndAcademicYearAsync(Guid scheduleId, AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task<Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task<bool> IsSubjectsExist(List<Guid> subjectsIds, CancellationToken cancellationToken = default);
    Task<Subject?> GetSubjectById(Guid subjectId);
    void DeleteSubjectAsync(Schedule schedule, Subject subject, CancellationToken cancellationToken = default);
    void AddSubject(Schedule schedule, Subject subject);
    void AddSchedule(Schedule schedule);
    Task UpdateScheduleSessionsAsync(Schedule schedule, List<Session> sessions, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateScheduleFileAsync(Schedule schedule, IFormFile scheduleFile, CancellationToken cancellationToken = default);
}