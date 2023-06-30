using System.Globalization;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Deadlines.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Deadline;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Deadlines.Commands;

internal sealed class UpdateDeadlinesCommandHandler : IRequestHandler<UpdateDeadlinesCommand, ErrorOr<UpdateDeadlinesResult>>
{
    private readonly ITimeService _timeService;
    private readonly IDeadlinesRepository _deadlinesRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDeadlinesCommandHandler(ITimeService timeService,
        IDeadlinesRepository deadlinesRepository,
        IUnitOfWork unitOfWork)
    {
        _timeService = timeService;
        _deadlinesRepository = deadlinesRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateDeadlinesResult>> Handle(UpdateDeadlinesCommand request, CancellationToken cancellationToken)
    {
        var deadlines = request.Deadlines
            .Select(x => Deadline.Create(
                x.Title,
                x.Description,
                _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.DueDate, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)),
                Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true)));
        
        await _deadlinesRepository.UpdateDeadlinesAsync(deadlines, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateDeadlinesResult(true);
    }
}