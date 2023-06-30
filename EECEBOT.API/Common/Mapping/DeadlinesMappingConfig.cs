using EECEBOT.Application.Deadlines.Commands;
using EECEBOT.Application.Deadlines.Queries.GetDeadlines;
using EECEBOT.Contracts.Deadlines;
using Mapster;

namespace EECEBOT.API.Common.Mapping;

public class DeadlinesMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<string, GetDeadlinesQuery>()
            .MapWith(src => new GetDeadlinesQuery(src));

        config.NewConfig<DeadlineRequest, UpdateDeadlineRequest>()
            .MapWith(src => new UpdateDeadlineRequest(
                src.Title,
                src.Description,
                src.DueDate));
        
        config.NewConfig<(UpdateDeadlinesRequest UpdateDeadlinesRequest, string academicYear), UpdateDeadlinesCommand>()
            .MapWith(src => new UpdateDeadlinesCommand(
                src.UpdateDeadlinesRequest.Deadlines.Adapt<List<UpdateDeadlineRequest>>(config),
                src.academicYear));
    }
}