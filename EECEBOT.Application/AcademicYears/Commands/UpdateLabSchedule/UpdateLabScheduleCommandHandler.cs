using System.Globalization;
using EECEBOT.Application.AcademicYears.ResultModels.LabScheduleResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLabSchedule;

internal sealed class UpdateLabScheduleCommandHandler : IRequestHandler<UpdateLabScheduleLabsCommand, ErrorOr<UpdateLabScheduleLabsResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly ITimeService _timeService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLabScheduleCommandHandler(
        IAcademicYearRepository academicYearRepository,
        ITimeService timeService,
        IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _timeService = timeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateLabScheduleLabsResult>> Handle(UpdateLabScheduleLabsCommand request, CancellationToken cancellationToken)
    {
       var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);
        
       if (academicYear is null)
           return Errors.AcademicYearErrors.AcademicYearNotFound;

       var labs = request.Labs.Select(x => Lab.Create(
           x.Name,
           _timeService.ConvertAppDateTimeToUtcDateTimeOffset(DateTime.ParseExact(x.Date, "dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)),
           x.Location,
           Enum.Parse<Section>(x.Section, ignoreCase: true),
           new Range(new Index(x.BenchNumbersRangeStart), new Index(x.BenchNumbersRangeEnd))))
           .ToList();

       var updateResult = academicYear.TryUpdateLabScheduleLabs(labs);
       
         if (updateResult.IsError)
              return updateResult.Errors;
       
       _unitOfWork.Update(academicYear);
       
       await _unitOfWork.SaveChangesAsync(cancellationToken);

       return new UpdateLabScheduleLabsResult(true);
    }
}