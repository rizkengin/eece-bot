using EECEBOT.Application.LabSchedules.Commands.CreateLabSchedule;
using EECEBOT.Application.LabSchedules.Commands.UpdateLabSchedule;
using EECEBOT.Application.LabSchedules.Commands.UpdateLabScheduleFile;
using EECEBOT.Application.LabSchedules.Queries.GetLabSchedule;
using EECEBOT.Contracts.LabSchedules;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace EECEBOT.API.Functions;

public class LabScheduleFunctions : BaseFunction
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public LabScheduleFunctions(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [Function("Create-Lab-Schedule")]
    public async Task<IActionResult> CreateLabSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{academicYear}/lab-schedule")] HttpRequest request,
        FunctionContext context,
        string academicYear)
    {
        var createLabScheduleRequest = await request.ReadFromJsonAsync<CreateLabScheduleRequest>();
        
        var command = _mapper.Map<CreateLabScheduleCommand>((academicYear, createLabScheduleRequest));
        
        var result = await _sender.Send(command, context.CancellationToken);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<CreateLabScheduleResponse>(success)),
            failure => Problem(failure, context.InvocationId));
    }
    
    [Function("Update-Lab-Schedule")]
    public async Task<IActionResult> UpdateLabSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/lab-schedule/{labScheduleId:Guid}")] HttpRequest request,
        FunctionContext context,
        string academicYear,
        Guid labScheduleId)
    {
        var updateLabScheduleRequest = await request.ReadFromJsonAsync<UpdateLabScheduleRequest>();
        
        var command = _mapper.Map<UpdateLabScheduleCommand>((labScheduleId, academicYear, updateLabScheduleRequest));
        
        var result = await _sender.Send(command, context.CancellationToken);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateLabScheduleResponse>(success)),
            failure => Problem(failure, context.InvocationId));
    }
    
    [Function("Get-Lab-Schedule")]
    public async Task<IActionResult> GetLabSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/lab-schedule")] HttpRequest request,
        FunctionContext context,
        string academicYear)
    {
        var query = _mapper.Map<GetLabScheduleQuery>(academicYear);
        
        var result = await _sender.Send(query, context.CancellationToken);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetLabScheduleResponse>(success)),
            failure => Problem(failure, context.InvocationId));
    }
    
    [Function("Update-Lab-Schedule-File")]
    public async Task<IActionResult> UpdateLabScheduleFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/lab-schedule/{labScheduleId:Guid}/file")] HttpRequest request,
        FunctionContext context,
        Guid labScheduleId,
        string academicYear)
    {
        if (!request.HasFormContentType)
        {
            return Problem(context.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }

        var file = request.Form.Files[0];
        
        var command = _mapper.Map<UpdateLabScheduleFileCommand>((labScheduleId, academicYear, file));
        
        var result = await _sender.Send(command, context.CancellationToken);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateLabScheduleFileResponse>(success)),
            failure => Problem(failure, context.InvocationId));
    }
}