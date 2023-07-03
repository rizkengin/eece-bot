using EECEBOT.Application.LabSchedules.Commands.CreateLabSchedule;
using EECEBOT.Application.LabSchedules.Commands.UpdateLabSchedule;
using EECEBOT.Application.LabSchedules.Commands.UpdateLabScheduleFile;
using EECEBOT.Application.LabSchedules.Queries.GetLabSchedule;
using EECEBOT.Contracts.LabSchedules;
using Mapster;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.API.Common.Mapping;

public class LabScheduleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<(string AcademicYear, CreateLabScheduleRequest createLabScheduleRequest), CreateLabScheduleCommand>()
            .MapWith(src => new CreateLabScheduleCommand(
                src.AcademicYear,
                src.createLabScheduleRequest.SplitMethod));
        
        config.NewConfig<LabRequest,LabUpdateRequest>()
            .MapWith(src => new LabUpdateRequest(
                src.Name,
                src.Date,
                src.Location,
                src.Section,
                src.BenchNumbersRangeStart,
                src.BenchNumbersRangeEnd));

        config
            .NewConfig<(Guid scheduleLabId, string academicYear, UpdateLabScheduleRequest updateLabScheduleRequest),
                UpdateLabScheduleCommand>()
            .MapWith(src => new UpdateLabScheduleCommand(
                src.scheduleLabId,
                src.academicYear,
                src.updateLabScheduleRequest.Labs.Adapt<IEnumerable<LabUpdateRequest>>(config)));

        config.NewConfig<string, GetLabScheduleQuery>()
            .MapWith(src => new GetLabScheduleQuery(src));

        config
            .NewConfig<(Guid labScheduleId, string academicYear, IFormFile labScheduleFile),
                UpdateLabScheduleFileCommand>()
            .MapWith(src => new UpdateLabScheduleFileCommand(
                src.labScheduleId,
                src.academicYear,
                src.labScheduleFile));
    }
}