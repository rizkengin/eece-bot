using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Link;

namespace EECEBOT.Application.Common.Persistence;

public interface ILinksRepository
{
    Task<IEnumerable<Link>> GetLinksByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default);
    Task UpdateLinksAsync(IEnumerable<Link> links, AcademicYear academicYear, CancellationToken cancellationToken = default);
}