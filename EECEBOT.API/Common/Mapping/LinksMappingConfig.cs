using EECEBOT.Application.AcademicYears.Commands.UpdateLinks;
using EECEBOT.Application.AcademicYears.Queries.GetLinks;
using EECEBOT.Contracts.Links;
using Mapster;

namespace EECEBOT.API.Common.Mapping;

public class LinksMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<string, GetLinksQuery>()
            .MapWith(src => new GetLinksQuery(src));
        
        config.NewConfig<LinkRequest, UpdateLinkRequest>()
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Url, src => src.Url);

        config.NewConfig<(UpdateLinksRequest updateLinksRequest, string academicYear), UpdateLinksCommand>()
            .MapWith(x 
                => new UpdateLinksCommand(x.updateLinksRequest.Links.Adapt<List<UpdateLinkRequest>>(config), x.academicYear));
    }
}