using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.DeleteSubject;

public sealed record DeleteScheduleSubjectCommand(string Year, Guid SubjectId) : IRequest<ErrorOr<DeleteScheduleSubjectCommandResult>>;