using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Schedules.Commands.CreateSubject;

public class CreateScheduleSubjectCommandValidator : AbstractValidator<CreateScheduleSubjectCommand>
{
    public CreateScheduleSubjectCommandValidator()
    {
        RuleFor(x => x.AcademicYear)
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
        
        RuleFor(x => x.ScheduleId)
            .NotEmpty()
            .WithMessage("Schedule id is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Subject name is required.");
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Subject code is required.");
    }
}