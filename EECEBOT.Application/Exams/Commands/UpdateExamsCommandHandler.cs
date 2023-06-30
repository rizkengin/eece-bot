using System.Globalization;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Exams.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Exam;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Exams.Commands;

public class UpdateExamsCommandHandler : IRequestHandler<UpdateExamsCommand, ErrorOr<UpdateExamsResult>>
{
    private readonly IExamsRepository _examsRepository;
    private readonly ITimeService _timeService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateExamsCommandHandler(IExamsRepository examsRepository,
        IUnitOfWork unitOfWork,
        ITimeService timeService)
    {
        _examsRepository = examsRepository;
        _unitOfWork = unitOfWork;
        _timeService = timeService;
    }

    public async Task<ErrorOr<UpdateExamsResult>> Handle(UpdateExamsCommand request, CancellationToken cancellationToken)
    {
        var exams = request.Exams
            .Select(x => Exam.Create(x.name,
                Enum.Parse<ExamType>(x.examType, ignoreCase: true),
                x.description,
                _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)),
                Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase:true),
                x.location))
            .ToList();
        
        await _examsRepository.UpdateExamsAsync(exams, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase:true), cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateExamsResult(true);
    }
}