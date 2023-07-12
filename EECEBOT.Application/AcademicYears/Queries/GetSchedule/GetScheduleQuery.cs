using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetSchedule;

public record GetScheduleQuery(string Year) : IRequest<ErrorOr<GetScheduleQueryResult>>;