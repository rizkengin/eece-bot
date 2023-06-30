using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Deadlines.ResultModels;
using EECEBOT.Domain.Common.Enums;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Deadlines.Queries.GetDeadlines;

internal sealed class GetDeadlinesQueryHandler : IRequestHandler<GetDeadlinesQuery, ErrorOr<GetDeadlinesQueryResult>>
{
    private readonly IDeadlinesRepository _deadlinesRepository;
    private readonly ITimeService _timeService;

    public GetDeadlinesQueryHandler(IDeadlinesRepository deadlinesRepository, ITimeService timeService)
    {
        _deadlinesRepository = deadlinesRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetDeadlinesQueryResult>> Handle(GetDeadlinesQuery request, CancellationToken cancellationToken)
    {
        var deadlines = await _deadlinesRepository.GetDeadlinesAsync(Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);

        return new GetDeadlinesQueryResult(deadlines.Select(x
            => new DeadlineResult(
                x.Id,
                x.Title,
                x.Description,
                _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.DueDate).ToString("dd-MM-yyyy HH:mm"))));
    }
}