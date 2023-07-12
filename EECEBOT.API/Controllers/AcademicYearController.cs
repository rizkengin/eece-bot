using EECEBOT.Application.AcademicYears.Commands.CreateLabSchedule;
using EECEBOT.Application.AcademicYears.Commands.CreateSchedule;
using EECEBOT.Application.AcademicYears.Commands.CreateSubject;
using EECEBOT.Application.AcademicYears.Commands.DeleteSubject;
using EECEBOT.Application.AcademicYears.Commands.UpdateDeadlines;
using EECEBOT.Application.AcademicYears.Commands.UpdateExams;
using EECEBOT.Application.AcademicYears.Commands.UpdateLabSchedule;
using EECEBOT.Application.AcademicYears.Commands.UpdateLabScheduleFile;
using EECEBOT.Application.AcademicYears.Commands.UpdateLinks;
using EECEBOT.Application.AcademicYears.Commands.UpdateSchedule;
using EECEBOT.Application.AcademicYears.Commands.UpdateScheduleFile;
using EECEBOT.Application.AcademicYears.Queries.GetDeadlines;
using EECEBOT.Application.AcademicYears.Queries.GetExams;
using EECEBOT.Application.AcademicYears.Queries.GetLabSchedule;
using EECEBOT.Application.AcademicYears.Queries.GetLinks;
using EECEBOT.Application.AcademicYears.Queries.GetSchedule;
using EECEBOT.Application.AcademicYears.Queries.GetSubjects;
using EECEBOT.Application.Authentication;
using EECEBOT.Contracts.Deadlines;
using EECEBOT.Contracts.Exams;
using EECEBOT.Contracts.LabSchedules;
using EECEBOT.Contracts.Links;
using EECEBOT.Contracts.Schedules;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EECEBOT.API.Controllers;

[Authorize(Policy = Policies.AcademicYearRepresentatives)]
[Route("academic-year/{year}")]
public class AcademicYearController : ApiController
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public AcademicYearController(
        ISender sender,
        IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [HttpGet("deadlines")]
    public async Task<IActionResult> GetDeadlines(string year)
    {
        var query = _mapper.Map<GetDeadlinesQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetDeadlinesResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("deadlines")]
    public async Task<IActionResult> UpdateDeadlines(string year, UpdateDeadlinesRequest request)
    {
        var command = _mapper.Map<UpdateDeadlinesCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateDeadlinesResponse>(success)), 
            Problem
        );
    }
    
    [HttpGet("exams")]
    public async Task<IActionResult> GetExams(string year)
    {
        var query = _mapper.Map<GetExamsQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetExamsResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("exams")]
    public async Task<IActionResult> UpdateExams(string year, UpdateExamsRequest request)
    {
        var command = _mapper.Map<UpdateExamsCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateExamsResponse>(success)), 
            Problem
        );
    }
    
    [HttpGet("links")]
    public async Task<IActionResult> GetLinks(string year)
    {
        var query = _mapper.Map<GetLinksQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetLinksResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("links")]
    public async Task<IActionResult> UpdateLinks(string year, UpdateLinksRequest request)
    {
        var command = _mapper.Map<UpdateLinksCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateLinksResponse>(success)), 
            Problem
        );
    }
    
    [HttpGet("schedule")]
    public async Task<IActionResult> GetSchedule(string year)
    {
        var query = _mapper.Map<GetScheduleQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetScheduleResponse>(success)), 
            Problem
        );
    }
    
    [HttpPost("schedule")]
    public async Task<IActionResult> CreateSchedule(string year, CreateScheduleRequest request)
    {
        var command = _mapper.Map<CreateScheduleCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<CreateScheduleResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("schedule")]
    public async Task<IActionResult> UpdateSchedule(string year, UpdateScheduleRequest request)
    {
        var command = _mapper.Map<UpdateScheduleCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateScheduleResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("schedule/file")]
    public async Task<IActionResult> UpdateScheduleFile([FromRoute] string year, [FromForm(Name = "file")] IFormFile file)
    {
        var command = _mapper.Map<UpdateScheduleFileCommand>((file, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateScheduleFileResponse>(success)), 
            Problem
        );
    }
    
    [HttpPost("schedule/subjects")]
    public async Task<IActionResult> CreateSubject(string year, CreateScheduleSubjectRequest request)
    {
        var command = _mapper.Map<CreateScheduleSubjectCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<CreateScheduleSubjectResponse>(success)), 
            Problem
        );
    }
    
    [HttpGet("schedule/subjects")]
    public async Task<IActionResult> GetSubjects(string year)
    {
        var query = _mapper.Map<GetScheduleSubjectsQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetScheduleSubjectsResponse>(success)), 
            Problem
        );
    }
    
    [HttpDelete("schedule/subjects/{id:guid}")]
    public async Task<IActionResult> DeleteSubject(string year, Guid id)
    {
        var command = _mapper.Map<DeleteScheduleSubjectCommand>((id, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<DeleteScheduleSubjectResponse>(success)), 
            Problem
        );
    }
    
    [HttpPost("lab-schedule")]
    public async Task<IActionResult> CreateLabSchedule(string year, CreateLabScheduleRequest request)
    {
        var command = _mapper.Map<CreateLabScheduleCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<CreateLabScheduleResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("lab-schedule/labs")]
    public async Task<IActionResult> UpdateLabSchedule(string year, UpdateLabScheduleLabsRequest request)
    {
        var command = _mapper.Map<UpdateLabScheduleLabsCommand>((request, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateLabScheduleLabsResponse>(success)), 
            Problem
        );
    }
    
    [HttpGet("lab-schedule")]
    public async Task<IActionResult> GetLabSchedule(string year)
    {
        var query = _mapper.Map<GetLabScheduleQuery>(year);
        
        var result = await _sender.Send(query);

        return result.Match(
            success => Ok(_mapper.Map<GetLabScheduleResponse>(success)), 
            Problem
        );
    }
    
    [HttpPut("lab-schedule/file")]
    public async Task<IActionResult> UpdateLabScheduleFile(string year, IFormFile file)
    {
        var command = _mapper.Map<UpdateLabScheduleFileCommand>((file, year));
        
        var result = await _sender.Send(command);

        return result.Match(
            success => Ok(_mapper.Map<UpdateLabScheduleFileResponse>(success)), 
            Problem
        );
    }
}