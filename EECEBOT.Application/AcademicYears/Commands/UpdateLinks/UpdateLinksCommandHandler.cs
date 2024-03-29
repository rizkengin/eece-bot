﻿using EECEBOT.Application.AcademicYears.ResultModels.LinksResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace EECEBOT.Application.AcademicYears.Commands.UpdateLinks;

internal sealed class UpdateLinksCommandHandler : IRequestHandler<UpdateLinksCommand, ErrorOr<UpdateLinksResult>>
{
    private readonly IAcademicYearRepository _academicYearRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLinksCommandHandler(IAcademicYearRepository academicYearRepository, IUnitOfWork unitOfWork)
    {
        _academicYearRepository = academicYearRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateLinksResult>> Handle(UpdateLinksCommand request, CancellationToken cancellationToken)
    {
        var academicYear = await _academicYearRepository.GetAcademicYearAsync(Enum.Parse<Year>(request.Year, ignoreCase: true), cancellationToken);

        if (academicYear is null)
            return Errors.AcademicYearErrors.AcademicYearNotFound;

        var links = request.Links
            .Select(x => Link.Create(x.Name, new Uri(x.Url)))
            .ToList();
        
        academicYear.UpdateLinks(links);
        
        await _unitOfWork.UpdateAsync(academicYear, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateLinksResult(true);
    }
}