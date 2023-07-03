using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.Schedules.Commands.DeleteSubject;

internal sealed class DeleteScheduleSubjectCommandHandler : IRequestHandler<DeleteScheduleSubjectCommand, ErrorOr<DeleteScheduleSubjectCommandResult>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public DeleteScheduleSubjectCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<DeleteScheduleSubjectCommandResult>> Handle(DeleteScheduleSubjectCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);

        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        var subject = schedule.Subjects.FirstOrDefault(x => x.Id == request.SubjectId);
        
        if (subject is null)
            return Errors.ScheduleErrors.SubjectNotFound;
        
        _scheduleRepository.DeleteSubjectAsync(schedule, subject, cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new DeleteScheduleSubjectCommandResult(true);
    }
}