using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Schedule.Entities;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Commands.CreateSubject;

internal sealed class CreateScheduleSubjectCommandHandler : IRequestHandler<CreateScheduleSubjectCommand, ErrorOr<CreateScheduleSubjectCommandResult>>
{
    private readonly IScheduleRepository _scheduleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateScheduleSubjectCommandHandler(IScheduleRepository scheduleRepository, IUnitOfWork unitOfWork)
    {
        _scheduleRepository = scheduleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<CreateScheduleSubjectCommandResult>> Handle(CreateScheduleSubjectCommand request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);

        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        if (schedule.Subjects.Any(x => string.Equals(x.Name, request.Name, StringComparison.InvariantCultureIgnoreCase) ||
                              string.Equals(x.Code, request.Code, StringComparison.InvariantCultureIgnoreCase)))
            return Errors.ScheduleErrors.SubjectAlreadyExists;

        var subject = Subject.Create(schedule.Id, request.Name, request.Code);
        
        _scheduleRepository.AddSubject(schedule, subject);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateScheduleSubjectCommandResult(true);
    }
}