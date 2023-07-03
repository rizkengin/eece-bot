using EECEBOT.Application.Schedules.ResultModels;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.DeleteSubject;

public sealed record DeleteScheduleSubjectCommand(Guid ScheduleId, Guid SubjectId) : IRequest<ErrorOr<DeleteScheduleSubjectCommandResult>>;