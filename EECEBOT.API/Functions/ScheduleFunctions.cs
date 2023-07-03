using EECEBOT.Application.Schedules.Commands.CreateSchedule;
using EECEBOT.Application.Schedules.Commands.CreateSubject;
using EECEBOT.Application.Schedules.Commands.DeleteSubject;
using EECEBOT.Application.Schedules.Commands.UpdateSchedule;
using EECEBOT.Application.Schedules.Commands.UpdateScheduleFile;
using EECEBOT.Application.Schedules.Queries.GetSchedule;
using EECEBOT.Application.Schedules.Queries.GetSubjects;
using EECEBOT.Contracts.Schedules;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace EECEBOT.API.Functions;

public class ScheduleFunctions : BaseFunction
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ScheduleFunctions(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [Function("Get-Schedule")]
    public async Task<IActionResult> GetSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/schedule")] HttpRequestData request,
        string academicYear)
    {
        var query = _mapper.Map<GetScheduleQuery>(academicYear);
        
        var result = await _sender.Send(query);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetScheduleResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Create-Schedule")]
    public async Task<IActionResult> CreateSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{academicYear}/schedule")] HttpRequestData request,
        string academicYear)
    {
        var body = await request.ReadAsStringAsync();

        var createScheduleRequest = JsonConvert.DeserializeObject<CreateScheduleRequest>(body!);
        
        if (createScheduleRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
        
        var command = _mapper.Map<CreateScheduleCommand>((academicYear, createScheduleRequest));
        
        var result = await _sender.Send(command);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<CreateScheduleResponse>(success)),
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Update-Schedule")]
    public async Task<IActionResult> UpdateSchedule(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/schedule/{scheduleId:Guid}")] HttpRequestData request,
        string academicYear, Guid scheduleId)
    {
        var body = await request.ReadAsStringAsync();

        var updateScheduleRequest = JsonConvert.DeserializeObject<UpdateScheduleRequest>(body!);
        
        if (updateScheduleRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
        
        var command = _mapper.Map<UpdateScheduleCommand>((scheduleId, academicYear, updateScheduleRequest));
        
        var result = await _sender.Send(command);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateScheduleResponse>(success)),
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Update-Schedule-File")]
    public async Task<IActionResult> UpdateScheduleFile(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/schedule/{scheduleId:Guid}/file")] HttpRequest request,
        FunctionContext context,
        Guid scheduleId,
        string academicYear)
    {
        if (!request.HasFormContentType)
        {
            return Problem(context.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
            
        var file = request.Form.Files[0];
        
        var command = _mapper.Map<UpdateScheduleFileCommand>((scheduleId, academicYear, file));
        
        var result = await _sender.Send(command);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateScheduleFileResponse>(success)),
            failure => Problem(failure, context.InvocationId)
        );
    }
    
    [Function("Create-Schedule-Subject")]
    public async Task<IActionResult> CreateScheduleSubject(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "{academicYear}/schedule/{scheduleId:Guid}/subjects")] HttpRequestData request,
        string academicYear, Guid scheduleId)
    {
        var body = await request.ReadAsStringAsync();

        var createScheduleSubjectRequest = JsonConvert.DeserializeObject<CreateScheduleSubjectRequest>(body!);
        
        if (createScheduleSubjectRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
        
        var command = _mapper.Map<CreateScheduleSubjectCommand>((scheduleId, academicYear, createScheduleSubjectRequest));
        
        var result = await _sender.Send(command);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<CreateScheduleSubjectResponse>(success)),
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Get-Schedule-Subjects")]
    public async Task<IActionResult> GetScheduleSubjects(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/schedule/{scheduleId:Guid}/subjects")] HttpRequestData request,
        string academicYear, Guid scheduleId)
    {
        var query = _mapper.Map<GetScheduleSubjectsQuery>(scheduleId);
        
        var result = await _sender.Send(query);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetScheduleSubjectsResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Delete-Schedule-Subject")]
    public async Task<IActionResult> DeleteScheduleSubject(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "{academicYear}/schedule/{scheduleId:Guid}/subjects/{subjectId:Guid}")] HttpRequestData request,
        string academicYear, Guid scheduleId, Guid subjectId)
    {
        var command = _mapper.Map<DeleteScheduleSubjectCommand>((scheduleId, subjectId));
        
        var result = await _sender.Send(command);
        
        return result.Match(
            success => new OkObjectResult(_mapper.Map<DeleteScheduleSubjectResponse>(success)),
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
}