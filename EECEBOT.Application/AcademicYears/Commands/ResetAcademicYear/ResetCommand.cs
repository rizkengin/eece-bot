using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.ResetAcademicYear;

public sealed record ResetCommand(string Year) : IRequest<ErrorOr<Deleted>>;