using System.Globalization;
using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Deadlines.Commands;

public class UpdateDeadlinesCommandValidator : AbstractValidator<UpdateDeadlinesCommand>
{
    public UpdateDeadlinesCommandValidator()
    {
        RuleFor(x=> x.Deadlines)
            .NotEmpty()
            .WithMessage("Deadlines are required.");

        RuleForEach(x => x.Deadlines)
            .Must(x => !string.IsNullOrWhiteSpace(x.Title))
            .WithMessage("Deadline name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Deadline description is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.DueDate))
            .WithMessage("Deadline date is required.")
            .Must(x => DateTime.TryParseExact(x.DueDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _));   
        
        RuleFor(x => x.AcademicYear)
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}