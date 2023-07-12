using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSchedule;


public sealed record UpdateScheduleCommand(
    string Year,
    string ScheduleStartDate,
    IEnumerable<SessionUpdateRequest> Sessions) : IRequest<ErrorOr<UpdateScheduleCommandResult>>;
    
public sealed record SessionUpdateRequest(
    string DayOfWeek,
    string Period,
    Guid SubjectId,
    string Lecturer,
    string Location,
    string SessionType,
    string Frequency,
    IEnumerable<string> Sections);