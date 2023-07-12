using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSubject;

public class CreateScheduleSubjectCommandValidator : AbstractValidator<CreateScheduleSubjectCommand>
{
    public CreateScheduleSubjectCommandValidator()
    {
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Subject name is required.");
        
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Subject code is required.");
    }
}