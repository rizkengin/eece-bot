using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.ResetAcademicYear;

internal sealed class ResetCommandHandler : IRequestHandler<ResetCommand, ErrorOr<Deleted>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResetCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Deleted>> Handle(ResetCommand request, CancellationToken cancellationToken)
    {
        var academicYear =
            await _academicYearRepository
                .GetAcademicYearAsync(Enum.Parse<Year>(request.Year), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        academicYear.Reset();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return new Deleted();
    }
}