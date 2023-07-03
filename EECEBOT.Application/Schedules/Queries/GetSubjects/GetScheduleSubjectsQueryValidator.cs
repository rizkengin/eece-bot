using FluentValidation;

namespace EECEBOT.Application.Schedules.Queries.GetSubjects;

public class GetScheduleSubjectsQueryValidator : AbstractValidator<GetScheduleSubjectsQuery>
{
    public GetScheduleSubjectsQueryValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule id is required.");
    }
}