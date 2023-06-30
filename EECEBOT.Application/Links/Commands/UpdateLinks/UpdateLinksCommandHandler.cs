using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Links.ResultModels;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Link;
using MediatR;
using ErrorOr;

namespace EECEBOT.Application.Links.Commands.UpdateLinks;

internal sealed class UpdateLinksCommandHandler : IRequestHandler<UpdateLinksCommand, ErrorOr<UpdateLinksResult>>
{
    private readonly ILinksRepository _linksRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLinksCommandHandler(ILinksRepository linksRepository, IUnitOfWork unitOfWork)
    {
        _linksRepository = linksRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<UpdateLinksResult>> Handle(UpdateLinksCommand request, CancellationToken cancellationToken)
    {
        var links = request.LinksTuples
            .Select(x => Link.Create(x.name, new Uri(x.url), Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase:true)))
            .ToList();
        
        await _linksRepository.UpdateLinksAsync(links, Enum.Parse<AcademicYear>(request.AcademicYear, ignoreCase:true), cancellationToken);
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateLinksResult(true);
    }
}