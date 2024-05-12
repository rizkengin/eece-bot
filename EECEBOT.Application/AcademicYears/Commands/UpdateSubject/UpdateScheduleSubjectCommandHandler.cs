using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSubject;

internal sealed class UpdateScheduleSubjectCommandHandler : IRequestHandler<UpdateScheduleSubjectCommand, ErrorOr<UpdateScheduleSubjectCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleSubjectCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateScheduleSubjectCommandResult>> Handle(UpdateScheduleSubjectCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        if (academicYear.Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        var updateResult = academicYear.TryUpdateScheduleSubject(request.SubjectId, request.Name, request.Code);

        if (updateResult.IsError)
            return updateResult.Errors;
        
        await _unitOfWork.UpdateAsync(academicYear, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateScheduleSubjectCommandResult(true);
    }
}