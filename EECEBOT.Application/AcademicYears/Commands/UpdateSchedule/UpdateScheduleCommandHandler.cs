using System.Globalization;
using EECEBOT.Application.AcademicYears.ResultModels.ScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateSchedule;

internal sealed class UpdateScheduleCommandHandler : IRequestHandler<UpdateScheduleCommand, ErrorOr<UpdateScheduleCommandResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateScheduleCommandHandler(
        IAcademicYearRepository academicYearRepository,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateScheduleCommandResult>> Handle(UpdateScheduleCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        var sessions = request.Sessions.Select(x => Session.Create(
                Enum.Parse<DayOfWeek>(x.DayOfWeek, ignoreCase: true),
            Enum.Parse<Period>(x.Period, ignoreCase: true),
            x.SubjectId,
            x.Lecturer,
            x.Location,
            Enum.Parse<SessionType>(x.SessionType, ignoreCase: true),
            Enum.Parse<SessionFrequency>(x.Frequency, ignoreCase: true),
            x.Sections.Select(y => Enum.Parse<Section>(y, ignoreCase: true)).ToList()))
            .ToList();

        var updateResult = academicYear
            .TryUpdateSchedule(DateOnly.FromDateTime(DateTime.ParseExact(request.ScheduleStartDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)),
            sessions,
            request.Sessions.Select(x => x.SubjectId).ToList());

        if (updateResult.IsError)
            return updateResult.Errors;

        _unitOfWork.Update(academicYear);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateScheduleCommandResult(true);
    }
}