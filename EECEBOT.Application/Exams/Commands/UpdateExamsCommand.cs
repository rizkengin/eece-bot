using EECEBOT.Application.Exams.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Exams.Commands;

public record UpdateExamsCommand(List<(string name,
    string examType,
    string description,
    string? location,
    string date)> Exams, string AcademicYear) : IRequest<ErrorOr<UpdateExamsResult>>;