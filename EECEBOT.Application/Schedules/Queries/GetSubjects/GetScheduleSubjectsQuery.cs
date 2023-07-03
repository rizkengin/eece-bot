using EECEBOT.Application.Schedules.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Queries.GetSubjects;

public sealed record GetScheduleSubjectsQuery(Guid ScheduleId) : IRequest<ErrorOr<GetScheduleSubjectsQueryResult>>;