using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule;
using EECEBOT.Domain.LabSchedule.ValueObjects;
using ErrorOr;
using Marten;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Infrastructure.Persistence;

public class LabScheduleRepository : ILabScheduleRepository
{
    private readonly IDocumentSession _documentSession;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "eece-bot";

    public LabScheduleRepository(IDocumentSession documentSession, BlobServiceClient blobServiceClient)
    {
        _documentSession = documentSession;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<LabSchedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<LabSchedule>()
            .FirstOrDefaultAsync(x => x.AcademicYear == academicYear, token: cancellationToken);
    }

    public async Task<LabSchedule?> GetByIdAndAcademicYearAsync(Guid labScheduleId, AcademicYear academicYear,
        CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<LabSchedule>()
            .FirstOrDefaultAsync(x => x.Id == labScheduleId && x.AcademicYear == academicYear, token: cancellationToken);
    }

    public void UpdateLabScheduleSessionsAsync(LabSchedule labSchedule, IEnumerable<Lab> labs)
    {
        labSchedule.UpdateLabs(labs);
        
        _documentSession.Update(labSchedule);
    }

    public async Task<ErrorOr<Updated>> UpdateLabScheduleFileAsync(LabSchedule labSchedule, IFormFile labScheduleFile,
        CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        if (labSchedule.FileUri is not null)
        {
            var oldBlobClient = blobContainerClient.GetBlobClient($"{Path.GetFileName(labSchedule.FileUri.LocalPath)}");
            
            await oldBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
        
        var newBlobClient = blobContainerClient.GetBlobClient($"{labSchedule.AcademicYear.ToFriendlyString()}_Lab_Schedule{Path.GetExtension(labScheduleFile.FileName)}");
        
        await newBlobClient.UploadAsync(labScheduleFile.OpenReadStream(), true, cancellationToken);
        
        var blobUri = newBlobClient.Uri;
        
        labSchedule.UpdateFileUri(blobUri);
        
        _documentSession.Update(labSchedule);
        
        return new Updated();
    }

    public void Add(LabSchedule labSchedule)
    {
        _documentSession.Insert(labSchedule);
    }
}