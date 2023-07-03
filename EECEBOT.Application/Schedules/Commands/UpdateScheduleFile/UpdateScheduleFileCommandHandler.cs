using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.UpdateScheduleFile;

internal sealed class UpdateScheduleFileCommandHandler : IRequestHandler<UpdateScheduleFileCommand, ErrorOr<UpdateScheduleFileCommandResult>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleFileCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateScheduleFileCommandResult>> Handle(UpdateScheduleFileCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository
            .GetByIdAndAcademicYearAsync(request.ScheduleId, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);

        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        var updateResult = await _scheduleRepository.UpdateScheduleFileAsync(schedule, request.ScheduleFile, cancellationToken);
        
        if (updateResult.IsError)
            return updateResult.Errors;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new UpdateScheduleFileCommandResult(true);
    }
}