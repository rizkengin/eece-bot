using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.ResetAcademicYear;

public sealed class ResetCommandValidator : AbstractValidator<ResetCommand>
{
    public ResetCommandValidator()
    {
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");
    }
}