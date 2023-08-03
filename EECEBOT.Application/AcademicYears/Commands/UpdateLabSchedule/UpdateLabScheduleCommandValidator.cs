using System.Globalization;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabSchedule;

public class UpdateLabScheduleCommandValidator : AbstractValidator<UpdateLabScheduleLabsCommand>
{
    public UpdateLabScheduleCommandValidator()
    {
        RuleFor(x => x.Labs)
            .NotNull()
            .WithMessage("Labs are required.");
        
        RuleForEach(x => x.Labs)
            .ChildRules(lab =>
            {
                lab.RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(50);
                
                lab.RuleFor(x => x.Date)
                    .NotEmpty()
                    .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    .WithMessage("Date format is invalid. Please use dd-MM-yyyy HH:mm format.");
                
                lab.RuleFor(x => x.Location)
                    .NotEmpty()
                    .MaximumLength(50);
                
                lab.RuleFor(x => x.Section)
                    .NotEmpty()
                    .MaximumLength(50)
                    .Must(x => Enum.TryParse<Section>(x, ignoreCase: true, out _))
                    .WithMessage("Invalid section type, Must be one of the following: SectionOne, SectionTwo, SectionThree, SectionFour");

                lab.RuleFor(x => x.BenchNumbersRangeStart)
                    .GreaterThanOrEqualTo(0)
                    .LessThan(x => x.BenchNumbersRangeEnd);

                lab.RuleFor(x => x.BenchNumbersRangeEnd)
                    .GreaterThan(x => x.BenchNumbersRangeStart);
            });

        RuleFor(x => x.Year)
            .NotEmpty()
            .Must(x => Enum.TryParse<Year>(x, ignoreCase: true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");
    }
}