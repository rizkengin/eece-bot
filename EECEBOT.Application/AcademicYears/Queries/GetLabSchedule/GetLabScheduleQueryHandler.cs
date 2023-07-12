using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetLabSchedule;

internal sealed class GetLabScheduleQueryHandler : IRequestHandler<GetLabScheduleQuery, ErrorOr<GetLabScheduleQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;
    
    public GetLabScheduleQueryHandler(IAcademicYearRepository academicYearRepository, ITimeService timeService)
    {
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetLabScheduleQueryResult>> Handle(GetLabScheduleQuery request, CancellationToken cancellationToken)
    {
        var labScheduleResult =
            await _academicYearRepository.GetLabScheduleAsync(
                Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (labScheduleResult.IsError)
            return labScheduleResult.Errors;

        if (labScheduleResult.Value is null)
            return Errors.LabScheduleErrors.LabScheduleNotFound;

        return new GetLabScheduleQueryResult(new LabScheduleResult(
            labScheduleResult.Value.Id,
            labScheduleResult.Value.SplitMethod.ToFriendlyString(),
            labScheduleResult.Value.FileUri?.ToString(),
            labScheduleResult.Value.Labs.Select(x => new LabResult(
                x.Name,
                _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.Date).ToString("dd-MM-yyyy HH:mm"),
                x.Location,
                x.Section.ToFriendlyString(),
                x.BenchNumbersRange.HasValue ? x.BenchNumbersRange.Value.Start.Value : 0,
                x.BenchNumbersRange.HasValue ? x.BenchNumbersRange.Value.End.Value : 0))));
    }
}