using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSubject;

public sealed class UpdateScheduleSubjectCommandValidator : AbstractValidator<UpdateScheduleSubjectCommand>
{
    public UpdateScheduleSubjectCommandValidator()
    {
        RuleFor(x => x.Year)
            .NotNull()
            .NotEmpty()
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");

        RuleFor(x => x.SubjectId)
            .NotNull()
            .NotEmpty()
            .Must(x => x != Guid.Empty)
            .WithMessage("Subject id is required");
        
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage("Subject name is required");

        RuleFor(x => x.Code)
            .NotNull()
            .NotEmpty()
            .WithMessage("Code is required");
    }
}