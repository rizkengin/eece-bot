using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Schedules.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Schedule.Enums;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Schedules.Queries.GetSchedule;

internal sealed class GetScheduleQueryHandler : IRequestHandler<GetScheduleQuery, ErrorOr<GetScheduleQueryResult>>
{
    private readonly IScheduleRepository _scheduleRepository;

    public GetScheduleQueryHandler(IScheduleRepository scheduleRepository)
    {
        _scheduleRepository = scheduleRepository;
    }

    public async Task<ErrorOr<GetScheduleQueryResult>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
    {
        var schedule = await _scheduleRepository.GetByAcademicYearAsync(Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);

        if (schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        return new GetScheduleQueryResult(
            new ScheduleResult(
                schedule.Id,
                schedule.ScheduleStartDate.ToString("dd-MM-yyyy"),
                schedule.FileUri?.ToString(),
                schedule.Sessions.Select(async s => new SessionResult(
                    await _scheduleRepository.GetSubjectById(s.SubjectId) is { } subject ?
                        new SubjectResult(subject.Name, subject.Code) :
                        new SubjectResult(string.Empty, string.Empty),
                    s.DayOfWeek.ToString(),
                    s.Period.ToString(),
                    s.Lecturer,
                    s.Location,
                    s.SessionType.ToString(),
                    s.Frequency.ToString(),
                    s.Sections.Select(sec => sec.ToFriendlyString())))
                    .Select(x => x.Result)));
    }
}