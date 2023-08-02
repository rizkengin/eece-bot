using EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateExams;

public record UpdateExamsCommand(List<UpdateExamRequest> Exams, string Year) : IRequest<ErrorOr<UpdateExamsResult>>;

public record UpdateExamRequest(string Name,
    string ExamType,
    string Description,
    string? Location,
    string Date);