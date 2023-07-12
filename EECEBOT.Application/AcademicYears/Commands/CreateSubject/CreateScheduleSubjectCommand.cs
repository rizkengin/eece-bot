using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSubject;

public sealed record CreateScheduleSubjectCommand(
    string Year,
    string Name,
    string Code) : IRequest<ErrorOr<CreateScheduleSubjectCommandResult>>; 