using System.Globalization;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.Enums;
using FluentValidation;

namespace EECEBOT.Application.LabSchedules.Commands.UpdateLabSchedule;

public class UpdateLabScheduleCommandValidator : AbstractValidator<UpdateLabScheduleCommand>
{
    public UpdateLabScheduleCommandValidator()
    {
        RuleForEach(x => x.Labs)
            .ChildRules(lab =>
            {
                lab.RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(50);
                
                lab.RuleFor(x => x.Date)
                    .NotEmpty()
                    .Must(x => DateTime.TryParseExact(x, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    .WithMessage("Date must be in the format dd-MM-yyyy HH:mm");
                
                lab.RuleFor(x => x.Location)
                    .NotEmpty()
                    .MaximumLength(50);
                
                lab.RuleFor(x => x.Section)
                    .NotEmpty()
                    .MaximumLength(50)
                    .Must(x => Enum.TryParse<Section>(x, ignoreCase: true, out _))
                    .WithMessage(
                        "Section must be one of the following: SectionOne, SectionTwo, SectionThree, SectionFour");

                lab.RuleFor(x => x.BenchNumbersRangeStart)
                    .GreaterThanOrEqualTo(0)
                    .LessThan(x => x.BenchNumbersRangeEnd)
                    .When(x => x.BenchNumbersRangeEnd > 0);

                lab.RuleFor(x => x.BenchNumbersRangeEnd)
                    .GreaterThan(x => x.BenchNumbersRangeStart)
                    .When(x => x.BenchNumbersRangeEnd > 0);
            });

        RuleFor(x => x.AcademicYear)
            .NotEmpty()
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase: true, out _))
            .WithMessage(
                "Academic year must be one of the following: FirstYear, SecondYear, ThirdYear, FourthYear");
        
        RuleFor(x => x.LabScheduleId)
            .NotEmpty()
            .WithMessage("Lab schedule id must not be empty");
    }
}