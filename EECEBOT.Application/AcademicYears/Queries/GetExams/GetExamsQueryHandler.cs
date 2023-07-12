using EECEBOT.Application.AcademicYears.ResultModels.ExamsResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetExams;

internal sealed class GetExamsQueryHandler : IRequestHandler<GetExamsQuery, ErrorOr<GetExamsQueryResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;

    public GetExamsQueryHandler(IAcademicYearRepository academicYearRepository, ITimeService timeService)
    {
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetExamsQueryResult>> Handle(GetExamsQuery request, CancellationToken cancellationToken)
    {
        var exams = await _academicYearRepository
            .GetExamsAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (exams.IsError)
            return exams.Errors;
        
        return new GetExamsQueryResult(exams.Value.Select(x => new ExamResult(x.Name,
            x.Type.ToString(),
            x.Description,
            x.Location,
            _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.Date).ToString("dd-MM-yyyy HH:mm"))));
    }
}