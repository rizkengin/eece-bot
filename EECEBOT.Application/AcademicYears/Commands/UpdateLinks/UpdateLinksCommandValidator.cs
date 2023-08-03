using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLinks;

public class UpdateLinksCommandValidator : AbstractValidator<UpdateLinksCommand>
{
    public UpdateLinksCommandValidator()
    {
        RuleFor(x => x.Links)
            .NotNull()
            .WithMessage("Links are required.");

        RuleForEach(x => x.Links)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Link name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Url))
            .WithMessage("Link url is required.")
            .Must(x => Uri.TryCreate(x.Url, UriKind.Absolute, out _))
            .WithMessage("Link url is invalid.");
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");
    }
}