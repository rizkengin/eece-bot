using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.LabSchedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.LabSchedule;
using EECEBOT.Domain.LabSchedule.Common;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Commands.CreateLabSchedule;

internal sealed class CreateLabScheduleCommandHandler : IRequestHandler<CreateLabScheduleCommand, ErrorOr<CreateLabScheduleCommandResult>>
{
    private readonly ILabScheduleRepository _labScheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLabScheduleCommandHandler(ILabScheduleRepository labScheduleRepository, IUnitOfWork unitOfWork)
    {
        _labScheduleRepository = labScheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CreateLabScheduleCommandResult>> Handle(CreateLabScheduleCommand request, CancellationToken cancellationToken)
    {
        var existingLabSchedule =
            await _labScheduleRepository.GetByAcademicYearAsync(
                Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);

        if (existingLabSchedule is not null)
            return Errors.LabScheduleErrors.LabScheduleAlreadyExists;

        var labSchedule = LabSchedule.Create(Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true),
            Enum.Parse<SplitMethod>(request.SplitMethod, ignoreCase: true));
        
        _labScheduleRepository.Add(labSchedule);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateLabScheduleCommandResult(true);
    }
}