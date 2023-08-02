using EECEBOT.Application.AcademicYears.Commands.UpdateExams;
using EECEBOT.Application.AcademicYears.Queries.GetExams;
using EECEBOT.Contracts.Exams;
using Mapster;

namespace EECEBOT.API.Common.Mapping;

public class ExamsMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<string, GetExamsQuery>()
            .MapWith(src => new GetExamsQuery(src));
        
        config.NewConfig<ExamRequest, UpdateExamRequest>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.ExamType, src => src.ExamType)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Location, src => src.Location)
            .Map(dest => dest.Date, src => src.Date);

        config.NewConfig<(UpdateExamsRequest UpdateExamsRequest, string academicYear), UpdateExamsCommand>()
            .MapWith(src => new UpdateExamsCommand(
                src.UpdateExamsRequest.Exams.Adapt<List<UpdateExamRequest>>(config),
                src.academicYear));
    }
}