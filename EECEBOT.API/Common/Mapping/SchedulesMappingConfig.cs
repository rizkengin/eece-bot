using EECEBOT.Application.Schedules.Commands.CreateSchedule;
using EECEBOT.Application.Schedules.Commands.UpdateSchedule;
using EECEBOT.Application.Schedules.Queries.GetSchedule;
using EECEBOT.Contracts.Schedules;
using Mapster;

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
                src.Subject.Adapt<SubjectUpdateRequest>(config),
                src.Lecturer,
                src.Location,
                src.SessionType,
                src.Frequency,
                src.Sections));

        config.NewConfig<(Guid ScheduleId, string AcademicYear, UpdateScheduleRequest UpdateScheduleRequest), UpdateScheduleCommand>()
            .MapWith(src => new UpdateScheduleCommand(
                src.ScheduleId,
                src.AcademicYear,
                src.UpdateScheduleRequest.ScheduleStartDate,
                src.UpdateScheduleRequest.Sessions.Adapt<IEnumerable<SessionUpdateRequest>>(config)));

        config.NewConfig<(string academicYear, CreateScheduleRequest createScheduleRequest), CreateScheduleCommand>()
            .MapWith(src => new CreateScheduleCommand(
                src.academicYear,
                src.createScheduleRequest.ScheduleStartDate));
    }
}