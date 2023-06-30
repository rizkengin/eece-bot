using System.Globalization;
using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleStartDate)
            .NotEmpty()
            .WithMessage("Schedule start date is required.")
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            .WithMessage("Schedule start date is invalid.");
        
        RuleFor(x => x.AcademicYear)
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}