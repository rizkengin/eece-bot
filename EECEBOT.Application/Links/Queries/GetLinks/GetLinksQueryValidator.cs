using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Links.Queries.GetLinks;

public class GetLinksQueryValidator : AbstractValidator<GetLinksQuery>
{
    public GetLinksQueryValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}