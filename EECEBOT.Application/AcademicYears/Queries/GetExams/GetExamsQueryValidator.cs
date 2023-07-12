using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Queries.GetExams;

public class GetExamsQueryValidator : AbstractValidator<GetExamsQuery>
{
    public GetExamsQueryValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}