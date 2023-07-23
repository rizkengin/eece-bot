using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateLabSchedule;

internal sealed class CreateLabScheduleCommandHandler : IRequestHandler<CreateLabScheduleCommand, ErrorOr<CreateLabScheduleCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLabScheduleCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<ErrorOr<CreateLabScheduleCommandResult>> Handle(CreateLabScheduleCommand request, CancellationToken cancellationToken)
    {
        var academicYear =
            await _academicYearRepository.GetAcademicYearAsync(
                Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        if (academicYear.LabSchedule is not null)
            return Errors.LabScheduleErrors.LabScheduleAlreadyExists;
        
        var labSchedule = LabSchedule.Create();
        
        academicYear.SetLabSchedule(labSchedule);
        
        _unitOfWork.Update(academicYear);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateLabScheduleCommandResult(true);
    }
}