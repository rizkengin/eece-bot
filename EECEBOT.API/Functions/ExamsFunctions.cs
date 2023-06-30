using EECEBOT.Application.Exams.Commands;
using EECEBOT.Application.Exams.Queries.GetExams;
using EECEBOT.Contracts.Exams;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;

namespace EECEBOT.API.Functions;

public class ExamsFunctions : BaseFunction
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public ExamsFunctions(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [Function("Get-Exams")]
    public async Task<IActionResult> GetExams([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/exams")] HttpRequestData request,
        string academicYear)
    {
        var query = _mapper.Map<GetExamsQuery>(academicYear);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetExamsResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Update-Exams")]
    public async Task<IActionResult> UpdateExams([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/exams")] HttpRequestData request,
        string academicYear)
    {
        var body = await request.ReadAsStringAsync();
        
        var examsRequest = JsonConvert.DeserializeObject<UpdateExamsRequest>(body!);
        
        if (examsRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
        
        var command = _mapper.Map<UpdateExamsCommand>((examsRequest, academicYear));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateExamsResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
}