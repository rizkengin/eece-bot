using EECEBOT.Application.LabSchedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Queries.GetLabSchedule;

public sealed record GetLabScheduleQuery(string AcademicYear) : IRequest<ErrorOr<GetLabScheduleQueryResult>>;