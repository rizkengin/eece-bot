using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabScheduleFile;

internal sealed class UpdateLabScheduleFileCommandHandler : IRequestHandler<UpdateLabScheduleFileCommand, ErrorOr<UpdateLabScheduleFileCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLabScheduleFileCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateLabScheduleFileCommandResult>> Handle(UpdateLabScheduleFileCommand request, CancellationToken cancellationToken)
    {
        var academicYear =
            await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true),
                cancellationToken);
        
        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;

        var updateResult =
            await _academicYearRepository.UpdateLabScheduleFileAsync(academicYear, request.LabScheduleFile,
                cancellationToken);

        if (updateResult.IsError)
            return updateResult.Errors;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new UpdateLabScheduleFileCommandResult(true);
    }
}