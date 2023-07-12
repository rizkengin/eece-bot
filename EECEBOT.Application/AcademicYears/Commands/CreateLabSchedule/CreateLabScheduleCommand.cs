using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateLabSchedule;

public sealed record CreateLabScheduleCommand(
    string Year, string SplitMethod) : IRequest<ErrorOr<CreateLabScheduleCommandResult>>;