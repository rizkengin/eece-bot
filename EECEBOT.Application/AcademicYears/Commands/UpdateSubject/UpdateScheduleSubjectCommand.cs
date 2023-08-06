using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSubject;

public sealed record UpdateScheduleSubjectCommand(
    string Year,
    Guid SubjectId,
    string Name,
    string Code) : IRequest<ErrorOr<UpdateScheduleSubjectCommandResult>>;