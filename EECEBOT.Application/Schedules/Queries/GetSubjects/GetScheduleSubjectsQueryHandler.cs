using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Errors;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Queries.GetSubjects;

public class GetScheduleSubjectsQueryHandler : IRequestHandler<GetScheduleSubjectsQuery, ErrorOr<GetScheduleSubjectsQueryResult>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleSubjectsQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ErrorOr<GetScheduleSubjectsQueryResult>> Handle(GetScheduleSubjectsQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByIdAsync(request.ScheduleId, cancellationToken);
        
        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        return new GetScheduleSubjectsQueryResult(schedule.Subjects
            .Select(x => new SubjectQueryResult(x.Id, x.Name, x.Code)));
    }
}