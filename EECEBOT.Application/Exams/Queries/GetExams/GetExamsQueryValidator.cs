using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Exams.Queries.GetExams;

public class GetExamsQueryValidator : AbstractValidator<GetExamsQuery>
{
    public GetExamsQueryValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}