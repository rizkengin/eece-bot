using EECEBOT.Application.LabSchedules.ResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.LabSchedules.Commands.UpdateLabSchedule;

public sealed record UpdateLabScheduleCommand(
    Guid LabScheduleId, 
    string AcademicYear, 
    IEnumerable<LabUpdateRequest> Labs) : IRequest<ErrorOr<UpdateLabScheduleResult>>;

public sealed record LabUpdateRequest(
    string Name,
    string Date,
    string Location,
    string Section,
    int BenchNumbersRangeStart,
    int BenchNumbersRangeEnd);