using System.Globalization;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.LabSchedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.LabSchedule.Common;
using EECEBOT.Domain.LabSchedule.ValueObjects;
using EECEBOT.Domain.Schedule.Enums;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Commands.UpdateLabSchedule;

internal sealed class UpdateLabScheduleCommandHandler : IRequestHandler<UpdateLabScheduleCommand, ErrorOr<UpdateLabScheduleResult>>
{
    private readonly ILabScheduleRepository _labScheduleRepository;
    private readonly ITimeService _timeService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLabScheduleCommandHandler(ILabScheduleRepository labScheduleRepository, IUnitOfWork unitOfWork, ITimeService timeService)
    {
        _labScheduleRepository = labScheduleRepository;
        _unitOfWork = unitOfWork;
        _timeService = timeService;
    }

    public async Task<ErrorOr<UpdateLabScheduleResult>> Handle(UpdateLabScheduleCommand request, CancellationToken cancellationToken)
    {
        var labSchedule = await _labScheduleRepository
            .GetByIdAndAcademicYearAsync(request.LabScheduleId, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);
        
        if (labSchedule is null)
            return Errors.LabScheduleErrors.LabScheduleNotFound;
        
        if (labSchedule.SplitMethod is SplitMethod.BySection && request.Labs.Any(x => x.BenchNumbersRangeEnd > 0))
            return Errors.LabScheduleErrors.LabScheduleSplitMethodBySectionButBenchNumbersRangeEndIsGreaterThanZero;
        
        if (labSchedule.SplitMethod is SplitMethod.ByBenchNumber && request.Labs.Any(x => x.BenchNumbersRangeEnd == 0))
            return Errors.LabScheduleErrors.LabScheduleSplitMethodByBenchNumberButBenchNumbersRangeEndIsZero;

        var labs = request.Labs.Select(x => Lab.Create(
            x.Name,
            _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.Date, "dd-MM-yyyy HH:mm",
                CultureInfo.InvariantCulture)),
            x.Location,
            Enum.Parse<Section>(x.Section, ignoreCase: true),
            x.BenchNumbersRangeEnd > 0
                ? new Range(new Index(x.BenchNumbersRangeStart), new Index(x.BenchNumbersRangeEnd))
                : null));

        _labScheduleRepository.UpdateLabScheduleSessionsAsync(labSchedule, labs);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateLabScheduleResult(true);
    }
}