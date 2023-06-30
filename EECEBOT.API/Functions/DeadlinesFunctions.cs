using EECEBOT.Application.Deadlines.Commands;
using EECEBOT.Application.Deadlines.Queries.GetDeadlines;
using EECEBOT.Contracts.Deadlines;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace EECEBOT.API.Functions;

public class DeadlinesFunctions : BaseFunction
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public DeadlinesFunctions(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [Function("Get-Deadlines")]
    public async Task<IActionResult> GetDeadlines([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/deadlines")] HttpRequestData request,
        string academicYear)
    {
        var query = _mapper.Map<GetDeadlinesQuery>(academicYear);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetDeadlinesResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Update-Deadlines")]
    public async Task<IActionResult> UpdateDeadlines([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/deadlines")] HttpRequestData request,
        string academicYear)
    {
        var body = await request.ReadAsStringAsync();
        
        var deadlinesRequest = JsonConvert.DeserializeObject<UpdateDeadlinesRequest>(body!);
        
        if (deadlinesRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
        
        var command = _mapper.Map<UpdateDeadlinesCommand>((deadlinesRequest, academicYear));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateDeadlinesResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
}