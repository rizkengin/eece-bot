using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Queries.GetLinks;

public class GetLinksQueryValidator : AbstractValidator<GetLinksQuery>
{
    public GetLinksQueryValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}