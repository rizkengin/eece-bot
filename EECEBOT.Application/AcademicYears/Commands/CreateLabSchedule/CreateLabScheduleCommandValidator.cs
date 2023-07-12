using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.CreateLabSchedule;

public class CreateLabScheduleCommandValidator : AbstractValidator<CreateLabScheduleCommand>
{
    public CreateLabScheduleCommandValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .WithMessage("Academic year is required.")
            .Must(x => Enum.TryParse<Year>(x, ignoreCase: true, out _))
            .WithMessage("Academic year is invalid.");
        
        RuleFor(x => x.SplitMethod)
            .NotEmpty()
            .WithMessage("Split method is required.")
            .Must(x => Enum.TryParse<SplitMethod>(x, ignoreCase: true, out _))
            .WithMessage("Split method is invalid.");
    }
}