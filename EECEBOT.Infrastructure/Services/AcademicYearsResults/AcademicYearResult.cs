using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Infrastructure.Services.AcademicYearsResults.Enums;

namespace EECEBOT.Infrastructure.Services.AcademicYearsResults;

public class AcademicYearResult
{
    private AcademicYearResult(
        Guid id,
        Year academicYear,
        ResultStatus lastResultStatus)
    {
        Id = id;
        AcademicYear = academicYear;
        LastResultStatus = lastResultStatus;
    }

    public Guid Id { get; set; } 
    public Year AcademicYear { get; set; }
    public ResultStatus LastResultStatus { get; set; }
    
    public static AcademicYearResult Create(
        Year academicYear,
        ResultStatus resultStatus) => new AcademicYearResult(Guid.NewGuid(), academicYear, resultStatus);
}