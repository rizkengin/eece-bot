using EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetExams;

public record GetExamsQuery(string Year) : IRequest<ErrorOr<GetExamsQueryResult>>;