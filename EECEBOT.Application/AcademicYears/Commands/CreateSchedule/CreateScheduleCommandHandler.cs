using System.Globalization;
using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSchedule;

internal sealed class CreateScheduleCommandHandler : IRequestHandler<CreateScheduleCommand, ErrorOr<CreateScheduleCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CreateScheduleCommandResult>> Handle(CreateScheduleCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;

        if (academicYear.Schedule is not null)
            return Errors.ScheduleErrors.ScheduleAlreadyExists;
        
        var scheduleResult = Schedule.TryCreate(DateOnly.FromDateTime(DateTime.ParseExact(request.ScheduleStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
        
        if (scheduleResult.IsError)
            return scheduleResult.Errors;
        
        academicYear.SetSchedule(scheduleResult.Value);
        
        await _unitOfWork.UpdateAsync(academicYear, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateScheduleCommandResult(true);
    }
}