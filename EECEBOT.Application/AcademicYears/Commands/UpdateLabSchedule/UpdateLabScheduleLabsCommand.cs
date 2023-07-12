using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabSchedule;

public sealed record UpdateLabScheduleLabsCommand(
    string Year, 
    IEnumerable<LabUpdateRequest> Labs) : IRequest<ErrorOr<UpdateLabScheduleLabsResult>>;

public sealed record LabUpdateRequest(
    string Name,
    string Date,
    string Location,
    string Section,
    int BenchNumbersRangeStart,
    int BenchNumbersRangeEnd);