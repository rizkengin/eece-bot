using System.Globalization;
using EECEBOT.Application.AcademicYears.ResultModels.DeadlinesResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateDeadlines;

internal sealed class UpdateDeadlinesCommandHandler : IRequestHandler<UpdateDeadlinesCommand, ErrorOr<UpdateDeadlinesResult>>
{
    private readonly ITimeService _timeService;
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDeadlinesCommandHandler(
        IAcademicYearRepository academicYearRepository,
        ITimeService timeService,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateDeadlinesResult>> Handle(UpdateDeadlinesCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true),
                cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        var deadlines = request.Deadlines
            .Select(x => Deadline.Create(
                x.Title,
                x.Description,
                _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.DueDate, "dd-MM-yyyy HH:mm",
                    CultureInfo.InvariantCulture))));
        
        academicYear.UpdateDeadlines(deadlines);
        
        await _unitOfWork.UpdateAsync(academicYear, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateDeadlinesResult(true);
    }
}