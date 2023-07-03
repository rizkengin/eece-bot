using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.LabSchedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.LabSchedule.Common;
using EECEBOT.Domain.Schedule.Enums;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Queries.GetLabSchedule;

internal sealed class GetLabScheduleQueryHandler : IRequestHandler<GetLabScheduleQuery, ErrorOr<GetLabScheduleQueryResult>>
{
    private readonly ILabScheduleRepository _labScheduleRepository;
    private readonly ITimeService _timeService;
    
    public GetLabScheduleQueryHandler(ILabScheduleRepository labScheduleRepository, ITimeService timeService)
    {
        _labScheduleRepository = labScheduleRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetLabScheduleQueryResult>> Handle(GetLabScheduleQuery request, CancellationToken cancellationToken)
    {
        var labSchedule =
            await _labScheduleRepository.GetByAcademicYearAsync(
                Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);

        if (labSchedule is null)
            return Errors.LabScheduleErrors.LabScheduleNotFound;

        return new GetLabScheduleQueryResult(new LabScheduleResult(
            labSchedule.Id,
            labSchedule.SplitMethod.ToFriendlyString(),
            labSchedule.FileUri?.ToString(),
            labSchedule.Labs.Select(x => new LabResult(
                x.Name,
                _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.Date).ToString("dd-MM-yyyy HH:mm"),
                x.Location,
                x.Section.ToFriendlyString(),
                x.BenchNumbersRange.HasValue ? x.BenchNumbersRange.Value.Start.Value : 0,
                x.BenchNumbersRange.HasValue ? x.BenchNumbersRange.Value.End.Value : 0))));
    }
}