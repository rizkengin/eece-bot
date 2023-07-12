using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Queries.GetLabSchedule;

public class GetLabScheduleQueryValidator : AbstractValidator<GetLabScheduleQuery>
{
    public GetLabScheduleQueryValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<Year>(x, ignoreCase: true, out _))
            .WithMessage("Academic year must be one of the following: FirstYear, SecondYear, ThirdYear, FourthYear");
    }
}