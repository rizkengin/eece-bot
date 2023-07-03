using FluentValidation;

namespace EECEBOT.Application.Schedules.Commands.DeleteSubject;

public class DeleteScheduleSubjectCommandValidator : AbstractValidator<DeleteScheduleSubjectCommand>
{
    public DeleteScheduleSubjectCommandValidator()
    {
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule id is required.");
        
        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage("Subject id is required.");
    }
}