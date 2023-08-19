using EECEBOT.Application.AcademicYears.Commands.ResetAcademicYear;
using Mapster;

namespace EECEBOT.API.Common.Mapping;

public class CommonMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<string, ResetCommand>()
            .MapWith(src => new ResetCommand(src));
    }
}