using System.Globalization;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Schedule;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.CreateSchedule;

internal sealed class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, ErrorOr<CreateScheduleCommandResult>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CreateScheduleCommandResult>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var academicYear = Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true);
        
        var existingSchedule = await _scheduleRepository
            .GetByAcademicYearAsync(academicYear, cancellationToken);

        if (existingSchedule is not null)
            return Errors.ScheduleErrors.ScheduleAlreadyExists;
        
        var scheduleResult = Schedule.TryCreate(academicYear, DateOnly.FromDateTime(DateTime.ParseExact(request.ScheduleStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
        
        if (scheduleResult.IsError)
            return scheduleResult.Errors;
        
        _scheduleRepository.AddSchedule(scheduleResult.Value);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateScheduleCommandResult(true);
    }
}