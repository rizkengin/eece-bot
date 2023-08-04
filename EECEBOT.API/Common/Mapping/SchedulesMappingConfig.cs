using EECEBOT.Application.AcademicYears.Commands.CreateSchedule;
using EECEBOT.Application.AcademicYears.Commands.CreateSubject;
using EECEBOT.Application.AcademicYears.Commands.DeleteSubject;
using EECEBOT.Application.AcademicYears.Commands.UpdateSchedule;
using EECEBOT.Application.AcademicYears.Commands.UpdateScheduleFile;
using EECEBOT.Application.AcademicYears.Commands.UpdateSubject;
using EECEBOT.Application.AcademicYears.Queries.GetSchedule;
using EECEBOT.Application.AcademicYears.Queries.GetSubjects;
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
                src.SubjectId,
                src.Lecturer,
                src.Location,
                src.SessionType,
                src.Frequency,
                src.Sections));

        config.NewConfig<(UpdateScheduleRequest updateScheduleRequest, string year), UpdateScheduleCommand>()
            .MapWith(src => new UpdateScheduleCommand(
                src.year,
                src.updateScheduleRequest.ScheduleStartDate,
                src.updateScheduleRequest.Sessions.Adapt<IEnumerable<SessionUpdateRequest>>(config)));

        config.NewConfig<(CreateScheduleRequest createScheduleRequest, string academicYear), CreateScheduleCommand>()
            .MapWith(src => new CreateScheduleCommand(
                src.academicYear,
                src.createScheduleRequest.ScheduleStartDate));

        config
            .NewConfig<(IFormFile scheduleFile, string year),
                UpdateScheduleFileCommand>()
            .MapWith(src => new UpdateScheduleFileCommand(
                src.year,
                src.scheduleFile));
        
        config.NewConfig<(CreateScheduleSubjectRequest createScheduleSubjectRequest, string year), CreateScheduleSubjectCommand>()
            .MapWith(src => new CreateScheduleSubjectCommand(
                src.year,
                src.createScheduleSubjectRequest.Name,
                src.createScheduleSubjectRequest.Code));

        config.NewConfig<string, GetScheduleSubjectsQuery>()
            .MapWith(x => new GetScheduleSubjectsQuery(x));
        
        config.NewConfig<(Guid subjectId, string year), DeleteScheduleSubjectCommand>()
            .MapWith(src => new DeleteScheduleSubjectCommand(
                src.year,
                src.subjectId));

        config
            .NewConfig<((UpdateScheduleSubjectRequest updateScheduleSubjectRequest, Guid subjectId), string year),
                UpdateScheduleSubjectCommand>()
            .MapWith(src => new UpdateScheduleSubjectCommand(
                src.year,
                src.Item1.subjectId,
                src.Item1.updateScheduleSubjectRequest.Name,
                src.Item1.updateScheduleSubjectRequest.Code));
    }
}