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
        
        config.NewConfig<ExamRequest, (string name, string examType, string description, string? location, string date)>()
            .Map(dest => dest.name, src => src.Name)
            .Map(dest => dest.examType, src => src.ExamType)
            .Map(dest => dest.description, src => src.Description)
            .Map(dest => dest.location, src => src.Location)
            .Map(dest => dest.date, src => src.Date);

        config.NewConfig<(UpdateExamsRequest UpdateExamsRequest, string academicYear), UpdateExamsCommand>()
            .MapWith(src 
                => new UpdateExamsCommand(src.UpdateExamsRequest.Exams.Adapt<List<(string name, string examType, string description, string? location, string date)>>(config)
                    , src.academicYear));
    }
}