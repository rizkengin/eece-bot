using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Exam;

namespace EECEBOT.Application.Common.Persistence;

public interface IExamsRepository
{
    Task<IEnumerable<Exam>> GetExamsAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task UpdateExamsAsync(IEnumerable<Exam> exams, AcademicYear academicYear, CancellationToken cancellationToken = default);
}