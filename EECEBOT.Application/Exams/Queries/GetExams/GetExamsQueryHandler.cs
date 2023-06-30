using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Application.Exams.ResultModels;
using EECEBOT.Domain.Common.Enums;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Exams.Queries.GetExams;

internal sealed class GetExamsQueryHandler : IRequestHandler<GetExamsQuery, ErrorOr<GetExamsResult>>
{
    private readonly IExamsRepository _examsRepository;
    private readonly ITimeService _timeService;

    public GetExamsQueryHandler(IExamsRepository examsRepository, ITimeService timeService)
    {
        _examsRepository = examsRepository;
        _timeService = timeService;
    }

    public async Task<ErrorOr<GetExamsResult>> Handle(GetExamsQuery request, CancellationToken cancellationToken)
    {
        var exams = await _examsRepository.GetExamsAsync(Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase: true), cancellationToken);
        
        return new GetExamsResult(exams.Select(x => new ExamResult(x.Name,
            x.Type.ToString(),
            x.Description,
            x.Location,
            _timeService.ConvertUtcDateTimeOffsetToAppDateTime(x.Date).ToString("dd-MM-yyyy HH:mm"))));
    }
}