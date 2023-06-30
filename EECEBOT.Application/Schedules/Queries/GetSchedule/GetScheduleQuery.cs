using EECEBOT.Application.Schedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.Schedules.Queries.GetSchedule;

public record GetScheduleQuery(string AcademicYear) : IRequest<ErrorOr<GetScheduleQueryResult>>;