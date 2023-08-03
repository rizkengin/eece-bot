using System.Globalization;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSchedule;

public class UpdateScheduleCommandValidator : AbstractValidator<UpdateScheduleCommand>
{
    public UpdateScheduleCommandValidator()
    {
        RuleFor(x => x.Sessions)
            .NotNull()
            .WithMessage("Sessions are required.");

        RuleForEach(x => x.Sessions)
            .Must(x => !string.IsNullOrWhiteSpace(x.DayOfWeek))
            .WithMessage("Day of week is required.")
            .Must(x => Enum.TryParse<DayOfWeek>(x.DayOfWeek, ignoreCase: true, out _))
            .WithMessage("Day of week is invalid.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Period))
            .WithMessage("Period is required.")
            .Must(x => Enum.TryParse<Period>(x.Period, ignoreCase: true, out _))
            .WithMessage("Invalid period type, Must be one of the following: FirstPeriod, SecondPeriod, ThirdPeriod, FourthPeriod, FifthPeriod, SixthPeriod")
            .Must(x => x.SubjectId != Guid.Empty)
            .WithMessage("SubjectId is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Lecturer))
            .WithMessage("Lecturer is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Location))
            .WithMessage("Location is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.SessionType))
            .WithMessage("Session type is required.")
            .Must(x => Enum.TryParse<SessionType>(x.SessionType, ignoreCase: true, out _))
            .WithMessage("Invalid session type, Must be one of the following: Lecture, Section, Other.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Frequency))
            .WithMessage("Frequency is required.")
            .Must(x => Enum.TryParse<SessionFrequency>(x.Frequency, ignoreCase: true, out _))
            .WithMessage("Invalid frequency type, Must be one of the following: Always, OddWeeks, EvenWeeks.")
            .Must(x => x.Sections.Any())
            .WithMessage("Sections are required.")
            .ChildRules(x => x.RuleForEach(y => y.Sections)
                .Must(s => Enum.TryParse<Section>(s, ignoreCase: true, out _))
                .WithMessage("Invalid section type, Must be one of the following: SectionOne, SectionTwo, SectionThree, SectionFour"));
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");

        RuleFor(x => x.ScheduleStartDate)
            .NotEmpty()
            .WithMessage("Schedule start date is required.")
            .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            .WithMessage("Date format is invalid. Please use dd-MM-yyyy format.");
    }
}