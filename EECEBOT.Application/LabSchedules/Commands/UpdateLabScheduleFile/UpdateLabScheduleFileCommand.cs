using EECEBOT.Application.LabSchedules.ResultModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using ErrorOr;

namespace EECEBOT.Application.LabSchedules.Commands.UpdateLabScheduleFile;

public sealed record UpdateLabScheduleFileCommand(Guid LabScheduleId,
    string AcademicYear,
    IFormFile LabScheduleFile) : IRequest<ErrorOr<UpdateLabScheduleFileCommandResult>>;