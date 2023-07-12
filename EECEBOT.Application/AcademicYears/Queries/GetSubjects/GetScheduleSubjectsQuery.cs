using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Queries.GetSubjects;

public sealed record GetScheduleSubjectsQuery(string Year) : IRequest<ErrorOr<GetScheduleSubjectsQueryResult>>;