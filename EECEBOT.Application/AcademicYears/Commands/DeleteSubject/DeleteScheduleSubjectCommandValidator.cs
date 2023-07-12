using EECEBOT.Domain.AcademicYearAggregate.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.DeleteSubject;

public class DeleteScheduleSubjectCommandValidator : AbstractValidator<DeleteScheduleSubjectCommand>
{
    public DeleteScheduleSubjectCommandValidator()
    {
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
        
        RuleFor(x => x.SubjectId)
            .NotEmpty()
            .WithMessage("Subject id is required.");
    }
}