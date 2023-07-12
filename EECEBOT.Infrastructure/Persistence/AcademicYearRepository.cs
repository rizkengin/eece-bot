using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.AcademicYearAggregate;
using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;
using Marten;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Infrastructure.Persistence;

public class AcademicYearRepository : IAcademicYearRepository
{
    private readonly IDocumentSession _documentSession;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "eece-bot";

    public AcademicYearRepository(IDocumentSession documentSession, BlobServiceClient blobServiceClient)
    {
        _documentSession = documentSession;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<AcademicYear?> GetAcademicYearAsync(Year year, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<AcademicYear>()
            .Where(ac => ac.Year == year)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ErrorOr<IEnumerable<Deadline>>> GetDeadlinesAsync(Year year, CancellationToken cancellationToken = default)
    { 
        var academicYear = await GetAcademicYearAsync(year, cancellationToken);
        
        if (academicYear is null) 
            return Errors.AcademicYearErrors.AcademicYearNotFound;
         
        return academicYear.Deadlines.ToArray();
    }

    public async Task<ErrorOr<IEnumerable<Exam>>> GetExamsAsync(Year year, CancellationToken cancellationToken = default)
    {
        var academicYear = await GetAcademicYearAsync(year, cancellationToken);
        
        if (academicYear is null) 
            return Errors.AcademicYearErrors.AcademicYearNotFound;
         
        return academicYear.Exams.ToArray();
    }

    public async Task<ErrorOr<IEnumerable<Link>>> GetLinksAsync(Year year, CancellationToken cancellationToken = default)
    {
        var academicYear = await GetAcademicYearAsync(year, cancellationToken);
        
        if (academicYear is null) 
            return Errors.AcademicYearErrors.AcademicYearNotFound;
         
        return academicYear.Links.ToArray();
    }

    public async Task<ErrorOr<Schedule?>> GetScheduleAsync(Year year, CancellationToken cancellationToken = default)
    {
        var academicYear = await GetAcademicYearAsync(year, cancellationToken);
        
        if (academicYear is null) 
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        return academicYear.Schedule;
    }

    public async Task<ErrorOr<LabSchedule?>> GetLabScheduleAsync(Year year, CancellationToken cancellationToken = default)
    {
        var academicYear = await GetAcademicYearAsync(year, cancellationToken);
        
        if (academicYear is null) 
            return Errors.AcademicYearErrors.AcademicYearNotFound;
        
        return academicYear.LabSchedule;
    }

    public async Task<ErrorOr<Updated>> UpdateScheduleFileAsync(AcademicYear academicYear, IFormFile scheduleFile,
        CancellationToken cancellationToken = default)
    {
        if (academicYear.Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        if (academicYear.Schedule.FileUri is not null)
        {
            var oldBlobClient = blobContainerClient.GetBlobClient($"{Path.GetFileName(academicYear.Schedule.FileUri.LocalPath)}");
            
            await oldBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }

        var newBlobClient = blobContainerClient.GetBlobClient($"{academicYear.Year.ToFriendlyString()}_Schedule{Path.GetExtension(scheduleFile.FileName)}");
        
        await newBlobClient.UploadAsync(scheduleFile.OpenReadStream(), true, cancellationToken);
        
        var blobUri = newBlobClient.Uri;
        
        academicYear.UpdateScheduleFileUri(blobUri);
        
        _documentSession.Update(academicYear);
        
        return new Updated();
    }

    public async Task<ErrorOr<Updated>> UpdateLabScheduleFileAsync(AcademicYear academicYear, IFormFile labScheduleFile,
        CancellationToken cancellationToken = default)
    {
        if (academicYear.LabSchedule is null)
            return Errors.LabScheduleErrors.LabScheduleNotFound;
        
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        
        if (academicYear.LabSchedule.FileUri is not null)
        {
            var oldBlobClient = blobContainerClient.GetBlobClient($"{Path.GetFileName(academicYear.LabSchedule.FileUri.LocalPath)}");
            
            await oldBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }
        
        var newBlobClient = blobContainerClient.GetBlobClient($"{academicYear.Year.ToFriendlyString()}_Lab_Schedule{Path.GetExtension(labScheduleFile.FileName)}");
        
        await newBlobClient.UploadAsync(labScheduleFile.OpenReadStream(), true, cancellationToken);
        
        var blobUri = newBlobClient.Uri;
        
        academicYear.UpdateLabScheduleFileUri(blobUri);
        
        _documentSession.Update(academicYear);
        
        return new Updated();
    }
}