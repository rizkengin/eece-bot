using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Schedules.Queries.GetSchedule;

public class GetScheduleQueryValidator : AbstractValidator<GetScheduleQuery>
{
    public GetScheduleQueryValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}