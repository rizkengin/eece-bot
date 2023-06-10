using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Schedule.ValueObjects;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Schedule;

public class Schedule
{
    private readonly List<Session> _sessions = new();
    
    [JsonConstructor]
    private Schedule(Guid id,
        StudyYear studyYear)
    {
        Id = id;
        StudyYear = studyYear;
    }
    public Guid Id { get; private set; }
    public StudyYear StudyYear { get; private set; }
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();

    public static Schedule Create(StudyYear studyYear) => new(Guid.NewGuid(), studyYear);
    
    public void UpdateSessions(IEnumerable<Session> sessions)
    {
        _sessions.Clear();
        _sessions.AddRange(sessions);
    }
}