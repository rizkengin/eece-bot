using EECEBOT.Application.Schedules.ResultModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.UpdateScheduleFile;

public sealed record UpdateScheduleFileCommand(Guid ScheduleId,
    string AcademicYear,
    IFormFile ScheduleFile) : IRequest<ErrorOr<UpdateScheduleFileCommandResult>>;