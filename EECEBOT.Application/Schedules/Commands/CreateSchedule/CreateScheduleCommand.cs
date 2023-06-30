using EECEBOT.Application.Schedules.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.CreateSchedule;

public sealed record CreateScheduleCommand(string AcademicYear,
    string ScheduleStartDate) : IRequest<ErrorOr<CreateScheduleCommandResult>>;