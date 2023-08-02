using System.Globalization;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Enums;
using FluentValidation;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateExams;

public class UpdateExamsCommandValidator : AbstractValidator<UpdateExamsCommand>
{
    public UpdateExamsCommandValidator()
    {
        RuleFor(x=> x.Exams)
            .NotNull()
            .WithMessage("Exams are required.");

        RuleForEach(x => x.Exams)
            .Must(x => !string.IsNullOrWhiteSpace(x.Name))
            .WithMessage("Exam name is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.ExamType))
            .WithMessage("Exam type is required.")
            .Must(x => Enum.TryParse<ExamType>(x.ExamType, ignoreCase: true, out _))
            .WithMessage("Exam type is invalid.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Exam description is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Date))
            .WithMessage("Exam date is required.")
            .Must(x => DateTime.TryParseExact(x.Date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            .WithMessage("Exam date format must be dd-MM-yyyy HH:mm.");
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Academic year is invalid.");
    }
}