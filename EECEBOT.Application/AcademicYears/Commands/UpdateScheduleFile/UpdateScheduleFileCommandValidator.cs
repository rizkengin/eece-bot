using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateScheduleFile;

public class UpdateScheduleFileCommandValidator : AbstractValidator<UpdateScheduleFileCommand>
{
    public UpdateScheduleFileCommandValidator()
    {
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");

        RuleFor(x => x.ScheduleFile)
            .NotEmpty()
            .WithMessage("Schedule file is required");
        
        RuleFor(x => x.ScheduleFile.ContentType)
            .NotEmpty()
            .WithMessage("Schedule file content type is required")
            .Must(x => x.Contains("image") || x.Contains("pdf"))
            .WithMessage("Schedule file must be an image or pdf");
        
        RuleFor(x => x.ScheduleFile.Length)
            .NotEmpty()
            .WithMessage("Schedule file length is required")
            .LessThanOrEqualTo(1024 * 1024 * 8)
            .WithMessage("Schedule file length must be less than or equal to 8MB");
    }
}