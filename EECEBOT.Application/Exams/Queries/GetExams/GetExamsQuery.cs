using EECEBOT.Application.Exams.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Exams.Queries.GetExams;

public record GetExamsQuery(string AcademicYear) : IRequest<ErrorOr<GetExamsResult>>;