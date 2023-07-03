using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule.Common;
using FluentValidation;

namespace EECEBOT.Application.LabSchedules.Commands.CreateLabSchedule;

public class CreateLabScheduleCommandValidator : AbstractValidator<CreateLabScheduleCommand>
{
    public CreateLabScheduleCommandValidator()
    {
        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase: true, out _))
            .WithMessage("Academic year is invalid.");
        
        RuleFor(x => x.SplitMethod)
            .NotEmpty()
            .WithMessage("Split method is required.")
            .Must(x => Enum.TryParse<SplitMethod>(x, ignoreCase: true, out _))
            .WithMessage("Split method is invalid.");
    }
}