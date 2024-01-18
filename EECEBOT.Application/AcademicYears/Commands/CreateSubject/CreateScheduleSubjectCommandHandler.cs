using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.CreateSubject;

internal sealed class CreateScheduleSubjectCommandHandler : IRequestHandler<CreateScheduleSubjectCommand, ErrorOr<CreateScheduleSubjectCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleSubjectCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _academicYearRepository = academicYearRepository;
    }

    public async Task<ErrorOr<CreateScheduleSubjectCommandResult>> Handle(CreateScheduleSubjectCommand request, CancellationToken cancellationToken)
    {
       var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        var subject = Subject.Create(request.Name, request.Code);
        
        var addResult = academicYear.TryAddScheduleSubject(subject);
        
        if (addResult.IsError)
            return addResult.Errors;

        await _unitOfWork.UpdateAsync(academicYear, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateScheduleSubjectCommandResult(true);
    }
}