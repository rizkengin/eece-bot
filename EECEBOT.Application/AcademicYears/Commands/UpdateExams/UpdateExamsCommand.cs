using EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateExams;

public record UpdateExamsCommand(List<(string name,
    string examType,
    string description,
    string? location,
    string date)> Exams, string Year) : IRequest<ErrorOr<UpdateExamsResult>>;