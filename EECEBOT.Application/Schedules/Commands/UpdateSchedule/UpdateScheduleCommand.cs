using EECEBOT.Application.Schedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.Schedules.Commands.UpdateSchedule;


public sealed record UpdateScheduleCommand(Guid ScheduleId,
    string AcademicYear,
    string ScheduleStartDate,
    IEnumerable<SessionUpdateRequest> Sessions) : IRequest<ErrorOr<UpdateScheduleCommandResult>>;
    
public sealed record SessionUpdateRequest(
    string DayOfWeek,
    string Period,
    SubjectUpdateRequest? Subject,
    string Lecturer,
    string Location,
    string SessionType,
    string Frequency,
    IEnumerable<string> Sections);
    
public sealed record SubjectUpdateRequest(string Name,
    string Code);