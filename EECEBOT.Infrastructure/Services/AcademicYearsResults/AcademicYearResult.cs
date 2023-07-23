using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Infrastructure.Services.AcademicYearsResults.Enums;

namespace EECEBOT.Infrastructure.Services.AcademicYearsResults;

public class AcademicYearResult
{
    public Year AcademicYear { get; set; }
    public ResultStatus LastResultStatus { get; set; }
}