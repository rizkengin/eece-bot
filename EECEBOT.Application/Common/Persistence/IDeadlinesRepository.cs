using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Deadline;

namespace EECEBOT.Application.Common.Persistence;

public interface IDeadlinesRepository
{
    Task<IEnumerable<Deadline>> GetDeadlinesAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task UpdateDeadlinesAsync(IEnumerable<Deadline> deadlines, AcademicYear academicYear, CancellationToken cancellationToken = default);
}