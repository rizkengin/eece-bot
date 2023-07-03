using EECEBOT.Application.LabSchedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Commands.CreateLabSchedule;

public sealed record CreateLabScheduleCommand(
    string AcademicYear, string SplitMethod) : IRequest<ErrorOr<CreateLabScheduleCommandResult>>;