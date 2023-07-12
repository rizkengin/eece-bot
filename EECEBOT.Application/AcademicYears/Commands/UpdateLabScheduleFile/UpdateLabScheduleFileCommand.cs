using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabScheduleFile;

public sealed record UpdateLabScheduleFileCommand(
    string Year,
    IFormFile LabScheduleFile) : IRequest<ErrorOr<UpdateLabScheduleFileCommandResult>>;