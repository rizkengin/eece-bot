using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule;
using EECEBOT.Domain.Schedule.Entities;
using ErrorOr;
using Marten;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Infrastructure.Persistence;

public class ScheduleRepository : IScheduleRepository
{
    private readonly IDocumentSession _documentSession;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "eece-bot";

    public ScheduleRepository(IDocumentSession documentSession, BlobServiceClient blobServiceClient)
    {
        _documentSession = documentSession;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<Schedule?> GetByIdAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.Id == scheduleId, token: cancellationToken);
    }

    public async Task<Schedule?> GetByIdAndAcademicYearAsync(Guid scheduleId, AcademicYear academicYear,
        CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.Id == scheduleId && x.AcademicYear == academicYear, token: cancellationToken);
    }

    public async Task<Schedule?> GetByAcademicYearAsync(AcademicYear academicYear, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Schedule>()
            .FirstOrDefaultAsync(x => x.AcademicYear == academicYear, token: cancellationToken);
    }

    public async Task<IEnumerable<Session>> GetScheduleSessions(List<Guid> sessionsIds, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Session>()
            .Where(x => sessionsIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSubjectsExist(List<Guid> subjectsIds, CancellationToken cancellationToken = default)
    {
        var subjects = await _documentSession.Query<Subject>()
            .Where(x => subjectsIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        return subjects.Count == subjectsIds.Count;
    }

    public Task<Subject?> GetSubjectById(Guid subjectId)
    {
        return _documentSession.Query<Subject>()
            .FirstOrDefaultAsync(x => x.Id == subjectId);
    }

    public void DeleteSubjectAsync(Schedule schedule, Subject subject, CancellationToken cancellationToken = default)
    {
        schedule.DeleteSubject(subject);
        
        _documentSession.Delete(subject);
        
        _documentSession.Update(schedule);
    }

    public async Task<IEnumerable<Subject>> GetSubjectsAsync(Guid scheduleId, CancellationToken cancellationToken = default)
    {
        return await _documentSession.Query<Subject>()
            .Where(x => x.ScheduleId == scheduleId)
            .ToListAsync(cancellationToken);
    }

    public void AddSubject(Schedule schedule, Subject subject)
    {
        schedule.AddSubject(subject);
        
        _documentSession.Insert(subject);
        
        _documentSession.Update(schedule);
    }

    public void AddSchedule(Schedule schedule)
    {
        _documentSession.Insert(schedule);
    }

    public async Task UpdateScheduleSessionsAsync(Schedule schedule, List<Session> sessions, CancellationToken cancellationToken = default)
    {
        var existingSessions = await _documentSession.Query<Session>()
            .Where(x => x.ScheduleId == schedule.Id)
            .ToListAsync(cancellationToken);
        
        foreach (var existingSession in existingSessions)
            _documentSession.Delete(existingSession);
        
        foreach (var session in sessions)
            _documentSession.Insert(session);
        
        schedule.UpdateSessions(sessions);
        
        _documentSession.Update(schedule);
    }

    public async Task<ErrorOr<Updated>> UpdateScheduleFileAsync(Schedule schedule, IFormFile scheduleFile, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        if (schedule.FileUri is not null)
        {
            var oldBlobClient = blobContainerClient.GetBlobClient($"{Path.GetFileName(schedule.FileUri.LocalPath)}");
            
            await oldBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: cancellationToken);
        }

        var newBlobClient = blobContainerClient.GetBlobClient($"{schedule.AcademicYear.ToFriendlyString()}_Schedule{Path.GetExtension(scheduleFile.FileName)}");
        
        await newBlobClient.UploadAsync(scheduleFile.OpenReadStream(), true, cancellationToken);
        
        var blobUri = newBlobClient.Uri;
        
        schedule.UpdateFileUri(blobUri);
        
        _documentSession.Update(schedule);
        
        return new Updated();
    }
}