using EECEBOT.Application.Links.Commands.UpdateLinks;
using EECEBOT.Application.Links.Queries.GetLinks;
using EECEBOT.Contracts.Links;
using Microsoft.AspNetCore.Http;
using MapsterMapper;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace EECEBOT.API.Functions;

public class LinksFunctions : BaseFunction
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public LinksFunctions(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [Function("Get-Links")]
    public async Task<IActionResult> GetLinks([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{academicYear}/links")] HttpRequestData request,
        string academicYear)
    {
        var query = _mapper.Map<GetLinksQuery>(academicYear);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => new OkObjectResult(_mapper.Map<GetLinksResponse>(success)), 
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
    
    [Function("Update-Links")]
    public async Task<IActionResult> UpdateLinks([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "{academicYear}/links")] HttpRequestData request,
        string academicYear)
    {
        var body = await request.ReadAsStringAsync();

        var linksRequest = JsonConvert.DeserializeObject<UpdateLinksRequest>(body!);
        
        if (linksRequest is null)
        {
            return Problem(request.FunctionContext.InvocationId, "Invalid request body",
                StatusCodes.Status400BadRequest);
        }
            
        var command = _mapper.Map<UpdateLinksCommand>((linksRequest, academicYear));
        
        var result = await _sender.Send(command);
    
        return result.Match(
            success => new OkObjectResult(_mapper.Map<UpdateLinksResponse>(success)),
            failure => Problem(failure, request.FunctionContext.InvocationId)
        );
    }
}