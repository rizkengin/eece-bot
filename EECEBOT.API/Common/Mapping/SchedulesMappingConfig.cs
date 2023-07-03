using EECEBOT.Application.Schedules.Commands.CreateSchedule;
using EECEBOT.Application.Schedules.Commands.CreateSubject;
using EECEBOT.Application.Schedules.Commands.DeleteSubject;
using EECEBOT.Application.Schedules.Commands.UpdateSchedule;
using EECEBOT.Application.Schedules.Commands.UpdateScheduleFile;
using EECEBOT.Application.Schedules.Queries.GetSchedule;
using EECEBOT.Application.Schedules.Queries.GetSubjects;
using EECEBOT.Contracts.Schedules;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.API.Common.Mapping;

public class SchedulesMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<string, GetScheduleQuery>()
            .MapWith(src => new GetScheduleQuery(src));

        config.NewConfig<SessionRequest, SessionUpdateRequest>()
            .MapWith(src => new SessionUpdateRequest(
                src.DayOfWeek,
                src.Period,
                src.SubjectId,
                src.Lecturer,
                src.Location,
                src.SessionType,
                src.Frequency,
                src.Sections));

        config.NewConfig<(Guid scheduleId, string academicYear, UpdateScheduleRequest updateScheduleRequest), UpdateScheduleCommand>()
            .MapWith(src => new UpdateScheduleCommand(
                src.scheduleId,
                src.academicYear,
                src.updateScheduleRequest.ScheduleStartDate,
                src.updateScheduleRequest.Sessions.Adapt<IEnumerable<SessionUpdateRequest>>(config)));

        config.NewConfig<(string academicYear, CreateScheduleRequest createScheduleRequest), CreateScheduleCommand>()
            .MapWith(src => new CreateScheduleCommand(
                src.academicYear,
                src.createScheduleRequest.ScheduleStartDate));

        config
            .NewConfig<(Guid scheduleId, string academicYear, IFormFile scheduleFile),
                UpdateScheduleFileCommand>()
            .MapWith(src => new UpdateScheduleFileCommand(
                src.scheduleId,
                src.academicYear,
                src.scheduleFile));
        
        config.NewConfig<(Guid scheduleId, string academicYear, CreateScheduleSubjectRequest createScheduleSubjectRequest), CreateScheduleSubjectCommand>()
            .MapWith(src => new CreateScheduleSubjectCommand(
                src.scheduleId,
                src.academicYear,
                src.createScheduleSubjectRequest.Name,
                src.createScheduleSubjectRequest.Code));

        config.NewConfig<Guid, GetScheduleSubjectsQuery>()
            .MapWith(x => new GetScheduleSubjectsQuery(x));
        
        config.NewConfig<(Guid scheduleId, Guid subjectId), DeleteScheduleSubjectCommand>()
            .MapWith(src => new DeleteScheduleSubjectCommand(
                src.scheduleId,
                src.subjectId));
    }
}