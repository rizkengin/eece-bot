using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetSchedule;

internal sealed class GetScheduleQueryHandler : IRequestHandler<GetScheduleQuery, ErrorOr<GetScheduleQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;

    public GetScheduleQueryHandler(IAcademicYearRepository academicYearRepository)
    {
        _academicYearRepository = academicYearRepository;
    }

    public async Task<ErrorOr<GetScheduleQueryResult>> Handle(GetScheduleQuery request, CancellationToken cancellationToken)
    {
        var academicYear =
            await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true),
                cancellationToken);

        if (academicYear is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        if (academicYear.Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        return new GetScheduleQueryResult(
            new ScheduleResult(
                    academicYear.Schedule.Id,
                    academicYear.Schedule.ScheduleStartDate.ToString("dd-MM-yyyy"),
                    academicYear.Schedule.FileUri?.ToString(),
                    academicYear.Schedule.Sessions.Select(s => new SessionResult(
                            academicYear.Schedule.Subjects.Single(x => x.Id == s.SubjectId) is { } subject ?
                        new SubjectResult(subject.Id, subject.Name, subject.Code) :
                        new SubjectResult(Guid.Empty, string.Empty, string.Empty),
                    s.DayOfWeek.ToString(),
                    s.Period.ToString(),
                    s.Lecturer,
                    s.Location,
                    s.SessionType.ToString(),
                    s.Frequency.ToString(),
                    s.Sections.Select(sec => sec.ToFriendlyString())))));
    }
}