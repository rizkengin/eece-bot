using EECEBOT.Application.AcademicYears.Commands.UpdateLabSchedule;
using EECEBOT.Application.AcademicYears.Commands.UpdateLabScheduleFile;
using EECEBOT.Application.AcademicYears.Queries.GetLabSchedule;
using EECEBOT.Contracts.LabSchedules;
using Mapster;

namespace EECEBOT.API.Common.Mapping;

public class LabScheduleMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<LabRequest,LabUpdateRequest>()
            .MapWith(src => new LabUpdateRequest(
                src.Name,
                src.Date,
                src.Location,
                src.Section,
                src.BenchNumbersRangeStart,
                src.BenchNumbersRangeEnd));

        config
            .NewConfig<(UpdateLabScheduleLabsRequest updateLabScheduleRequest, string year),
                UpdateLabScheduleLabsCommand>()
            .MapWith(src => new UpdateLabScheduleLabsCommand(
                src.year,
                src.updateLabScheduleRequest.Labs.Adapt<IEnumerable<LabUpdateRequest>>(config)));

        config.NewConfig<string, GetLabScheduleQuery>()
            .MapWith(src => new GetLabScheduleQuery(src));

        config
            .NewConfig<(string year, IFormFile labScheduleFile),
                UpdateLabScheduleFileCommand>()
            .MapWith(src => new UpdateLabScheduleFileCommand(
                src.year,
                src.labScheduleFile));
    }
}