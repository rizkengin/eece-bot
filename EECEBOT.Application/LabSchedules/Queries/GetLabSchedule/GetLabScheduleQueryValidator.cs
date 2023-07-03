using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.LabSchedules.Queries.GetLabSchedule;

public class GetLabScheduleQueryValidator : AbstractValidator<GetLabScheduleQuery>
{
    public GetLabScheduleQueryValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase: true, out _))
            .WithMessage("Academic year must be one of the following: FirstYear, SecondYear, ThirdYear, FourthYear");
    }
}