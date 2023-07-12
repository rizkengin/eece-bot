using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetSubjects;

public class GetScheduleSubjectsQueryHandler : IRequestHandler<GetScheduleSubjectsQuery, ErrorOr<GetScheduleSubjectsQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;

    public GetScheduleSubjectsQueryHandler(IAcademicYearRepository academicYearRepository)
    {
        _academicYearRepository = academicYearRepository;
    }

    public async Task<ErrorOr<GetScheduleSubjectsQueryResult>> Handle(GetScheduleSubjectsQuery request, CancellationToken cancellationToken)
    {
        var scheduleResult = await _academicYearRepository
            .GetScheduleAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (scheduleResult.IsError)
            return scheduleResult.Errors;
        
        if (scheduleResult.Value is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        return new GetScheduleSubjectsQueryResult(scheduleResult.Value.Subjects
            .Select(x => new SubjectQueryResult(x.Id, x.Name, x.Code)));
    }
}