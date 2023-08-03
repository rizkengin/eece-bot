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
            .WithMessage("Invalid exam type, must be one of the following: quiz, midterm, final.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Description))
            .WithMessage("Exam description is required.")
            .Must(x => !string.IsNullOrWhiteSpace(x.Date))
            .WithMessage("Exam date is required.")
            .Must(x => DateTime.TryParseExact(x.Date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out _))
            .WithMessage("Date format is invalid. Please use dd-MM-yyyy HH:mm format.");
        
        RuleFor(x => x.Year)
            .Must(x => Enum.TryParse<Year>(x, ignoreCase:true, out _))
            .WithMessage("Invalid academic year, must be one of the following: firstyear, secondyear, thirdyear, fourthyear.");
    }
}