using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Deadlines.Queries.GetDeadlines;

public class GetDeadlinesQueryValidator : AbstractValidator<GetDeadlinesQuery>
{
    public GetDeadlinesQueryValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}