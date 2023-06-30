using EECEBOT.Application.Schedules.Commands.CreateSchedule;
using EECEBOT.Application.Schedules.Commands.UpdateSchedule;
using EECEBOT.Application.Schedules.Queries.GetSchedule;
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
}