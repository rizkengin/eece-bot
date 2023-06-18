using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.ValueObjects;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Schedule;

public class Schedule
{
    private readonly List<Session> _sessions = new();
    
    [JsonConstructor]
    private Schedule(Guid id,
        AcademicYear academicYear)
    {
        Id = id;
        AcademicYear = academicYear;
    }
    public Guid Id { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    public Uri? FileUri { get; private set; }
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    public static Schedule Create(AcademicYear academicYear) => new(Guid.NewGuid(), academicYear);
    
    public void UpdateSessions(IEnumerable<Session> sessions)
    {
        _sessions.Clear();
        _sessions.AddRange(sessions);
    }
    
    public void UpdateFileUri(Uri fileUri) => FileUri = fileUri;
}