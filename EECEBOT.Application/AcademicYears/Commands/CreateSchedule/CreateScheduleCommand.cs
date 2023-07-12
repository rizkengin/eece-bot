using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSchedule;

public sealed record CreateScheduleCommand(
    string Year,
    string ScheduleStartDate) : IRequest<ErrorOr<CreateScheduleCommandResult>>;