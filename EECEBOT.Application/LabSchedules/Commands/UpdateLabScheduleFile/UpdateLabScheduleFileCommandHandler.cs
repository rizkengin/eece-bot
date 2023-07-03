using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.LabSchedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Commands.UpdateLabScheduleFile;

internal sealed class UpdateLabScheduleFileCommandHandler : IRequestHandler<UpdateLabScheduleFileCommand, ErrorOr<UpdateLabScheduleFileCommandResult>>
{
    private readonly ILabScheduleRepository _labScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLabScheduleFileCommandHandler(ILabScheduleRepository labScheduleRepository, IUnitOfWork unitOfWork)
    {
        _labScheduleRepository = labScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateLabScheduleFileCommandResult>> Handle(UpdateLabScheduleFileCommand request, CancellationToken cancellationToken)
    {
        var labSchedule = await _labScheduleRepository
            .GetByIdAndAcademicYearAsync(request.LabScheduleId,
                Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);
        
        if (labSchedule is null)
            return Errors.LabScheduleErrors.LabScheduleNotFound;

        var updateResult =
            await _labScheduleRepository.UpdateLabScheduleFileAsync(labSchedule, request.LabScheduleFile,
                cancellationToken);

        if (updateResult.IsError)
            return updateResult.Errors;
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new UpdateLabScheduleFileCommandResult(true);
    }
}