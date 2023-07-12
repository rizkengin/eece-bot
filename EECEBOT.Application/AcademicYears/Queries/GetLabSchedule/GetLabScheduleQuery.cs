using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetLabSchedule;

public sealed record GetLabScheduleQuery(string Year) : IRequest<ErrorOr<GetLabScheduleQueryResult>>;