using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabScheduleFile;

public class UpdateLabScheduleFileCommandValidator : AbstractValidator<UpdateLabScheduleFileCommand>
{
    public UpdateLabScheduleFileCommandValidator()
    {
        RuleFor(x => x.Year)
            .NotEmpty()
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");

        RuleFor(x => x.LabScheduleFile.ContentType)
            .NotEmpty()
            .WithMessage("Schedule file content type is required")
            .Must(x => x.Contains("image") || x.Contains("pdf"))
            .WithMessage("Schedule file must be an image or pdf");
        
        RuleFor(x => x.LabScheduleFile.Length)
            .NotEmpty()
            .WithMessage("Schedule file length is required")
            .LessThanOrEqualTo(1024 * 1024 * 8)
            .WithMessage("Schedule file length must be less than or equal to 8MB");
    }
}