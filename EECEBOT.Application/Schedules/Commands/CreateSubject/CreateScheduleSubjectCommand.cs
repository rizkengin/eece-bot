using EECEBOT.Application.Schedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.Schedules.Commands.CreateSubject;

public sealed record CreateScheduleSubjectCommand(
    Guid ScheduleId,
    string AcademicYear,
    string Name,
    string Code) : IRequest<ErrorOr<CreateScheduleSubjectCommandResult>>; 