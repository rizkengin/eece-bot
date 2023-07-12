using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLinks;

public class UpdateLinksCommandValidator : AbstractValidator<UpdateLinksCommand>
{
    public UpdateLinksCommandValidator()
    {
        RuleFor(x => x.LinksTuples)
            .NotEmpty()
            .WithMessage("Links are required.");

        RuleForEach(x => x.LinksTuples)
            .Must(x => !string.IsNullOrWhiteSpace(x.name))
            .WithMessage("Link name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.url))
            .WithMessage("Link url is required.")
            .Must(x => Uri.TryCreate(x.url, UriKind.Absolute, out _))
            .WithMessage("Link url is invalid.");
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}