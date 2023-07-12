using System.Globalization;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSchedule;

public class CreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>
{
    public CreateScheduleCommandValidator()
    {
        RuleFor(x => x.ScheduleStartDate)
            .NotEmpty()
            .WithMessage("Schedule start date is required.")
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            .WithMessage("Schedule start date is invalid.");
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}