using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Queries.GetSubjects;

public class GetScheduleSubjectsQueryValidator : AbstractValidator<GetScheduleSubjectsQuery>
{
    public GetScheduleSubjectsQueryValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}