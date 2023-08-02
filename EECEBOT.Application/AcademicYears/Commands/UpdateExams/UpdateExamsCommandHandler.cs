using System.Globalization;
using EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateExams;

public class UpdateExamsCommandHandler : IRequestHandler<UpdateExamsCommand, ErrorOr<UpdateExamsResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExamsCommandHandler(IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork,
        ITimeService timeService)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
        _timeService = timeService;
    }

    public async Task<ErrorOr<UpdateExamsResult>> Handle(UpdateExamsCommand request, CancellationToken cancellationToken)
    {
        var academicYear =
            await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        var exams = request.Exams
            .Select(x => Exam.Create(x.Name,
                Enum.Parse<ExamType>(x.ExamType, ignoreCase: true),
                x.Description,
                _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.Date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)),
                x.Location))
            .ToList();

        academicYear.UpdateExams(exams);
        
        _unitOfWork.Update(academicYear);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateExamsResult(true);
    }
}