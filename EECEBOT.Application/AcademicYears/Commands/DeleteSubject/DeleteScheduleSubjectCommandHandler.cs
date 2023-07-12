using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.DeleteSubject;

internal sealed class DeleteScheduleSubjectCommandHandler : IRequestHandler<DeleteScheduleSubjectCommand, ErrorOr<DeleteScheduleSubjectCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteScheduleSubjectCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<DeleteScheduleSubjectCommandResult>> Handle(DeleteScheduleSubjectCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        var deleteResult = academicYear.TryDeleteScheduleSubject(request.SubjectId);
        
        if (deleteResult.IsError)
            return deleteResult.Errors;
        
        _unitOfWork.Update(academicYear);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteScheduleSubjectCommandResult(true);
    }
}