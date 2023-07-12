using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateScheduleFile;

public sealed record UpdateScheduleFileCommand(
    string Year,
    IFormFile ScheduleFile) : IRequest<ErrorOr<UpdateScheduleFileCommandResult>>;