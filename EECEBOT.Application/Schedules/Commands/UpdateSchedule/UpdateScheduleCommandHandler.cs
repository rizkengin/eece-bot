using System.Globalization;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Schedule.Entities;
using EECEBOT.Domain.Schedule.Enums;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.UpdateSchedule;

internal sealed class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, ErrorOr<UpdateScheduleCommandResult>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateScheduleCommandResult>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository
            .GetByIdAndAcademicYearAsync(request.ScheduleId, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);
        
        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        if (!await _scheduleRepository.IsSubjectsExist(request.Sessions.Select(x => x.SubjectId).Distinct().ToList(), cancellationToken))
            return Errors.ScheduleErrors.InvalidSubjectsIds;

        var sessions = request.Sessions.Select(x => Session.Create(
            schedule.Id,
            Enum.Parse<DayOfWeek>(x.DayOfWeek, ignoreCase: true),
            Enum.Parse<Period>(x.Period, ignoreCase: true),
            x.SubjectId,
            x.Lecturer,
            x.Location,
            Enum.Parse<SessionType>(x.SessionType, ignoreCase: true),
            Enum.Parse<SessionFrequency>(x.Frequency, ignoreCase: true),
            x.Sections.Select(y => Enum.Parse<Section>(y, ignoreCase: true)).ToList()))
            .ToList();

        var updateScheduleStartDateResult = schedule
            .TryUpdateScheduleStartDate(DateOnly.FromDateTime(DateTime.ParseExact(request.ScheduleStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
        
        if (updateScheduleStartDateResult.IsError)
            return updateScheduleStartDateResult.Errors;

        await _scheduleRepository.UpdateScheduleSessionsAsync(schedule, sessions, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateScheduleCommandResult(true);
    }
}