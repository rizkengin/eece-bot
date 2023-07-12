using EECEBOT.Application.AcademicYears.ResultModels.DeadlinesResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetDeadlines;

internal sealed class GetDeadlinesQueryHandler : IRequestHandler<GetDeadlinesQuery, ErrorOr<GetDeadlinesQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;

    public GetDeadlinesQueryHandler(IAcademicYearRepository academicYearRepository, ITimeService timeService)
    {
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetDeadlinesQueryResult>> Handle(GetDeadlinesQuery request, CancellationToken cancellationToken)
    {
        var deadlinesResult =
            await _academicYearRepository.GetDeadlinesAsync(
                Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (deadlinesResult.IsError)
            return deadlinesResult.Errors;

        return new GetDeadlinesQueryResult(deadlinesResult.Value.Select(x
            => new DeadlineResult(
                x.Id,
                x.Title,
                x.Description,
                _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.DueDate).ToString("dd-MM-yyyy HH:mm"))));
    }
}