using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateScheduleFile;

internal sealed class UpdateScheduleFileCommandHandler : IRequestHandler<UpdateScheduleFileCommand, ErrorOr<UpdateScheduleFileCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleFileCommandHandler(IAcademicYearRepository academicYearRepository, IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateScheduleFileCommandResult>> Handle(UpdateScheduleFileCommand request, CancellationToken cancellationToken)
    {
       var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        if (academicYear.Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        var updateResult = await _academicYearRepository.UpdateScheduleFileAsync(academicYear, request.ScheduleFile, cancellationToken);
        
        if (updateResult.IsError)
            return updateResult.Errors;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new UpdateScheduleFileCommandResult(true);
    }
}