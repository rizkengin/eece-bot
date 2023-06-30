using System.Globalization;
using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.Exams.Commands;

public class UpdateExamsCommandValidator : AbstractValidator<UpdateExamsCommand>
{
    public UpdateExamsCommandValidator()
    {
        RuleFor(x=> x.Exams)
            .NotEmpty()
            .WithMessage("Exams are required.");
        
        RuleForEach(x => x.Exams)
            .Must(x => !string.IsNullOrWhiteSpace(x.name))
            .WithMessage("Exam name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.examType))
            .WithMessage("Exam type is required.")
            .Must(x=> Enum.TryParse<ExamType>(x.examType, ignoreCase:true, out _))
            .WithMessage("Exam type is invalid.")
            .Must(x => !string.IsNullOrWhiteSpace(x.description))
            .WithMessage("Exam description is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.date))
            .WithMessage("Exam date is required.")
            .Must(x => DateTime.TryParseExact(x.date,"dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            .WithMessage("Exam date is invalid.");
        
        RuleFor(x => x.AcademicYear)
            .Must(x => Enum.TryParse<AcademicYear>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}