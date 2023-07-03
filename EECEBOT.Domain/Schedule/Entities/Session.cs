using EECEBOT.Domain.Schedule.Enums;

namespace EECEBOT.Domain.Schedule.Entities;

public class Session
{
    private Session(Guid id,
        Guid scheduleId,
        DayOfWeek dayOfWeek,
        Period period,
        Guid subjectId,
        string lecturer,
        string location,
        SessionType sessionType,
        SessionFrequency frequency,
        List<Section> sections)
    {
        Id = id;
        ScheduleId = scheduleId;
        DayOfWeek = dayOfWeek;
        Period = period;
        SubjectId = subjectId;
        Lecturer = lecturer;
        Location = location;
        SessionType = sessionType;
        Frequency = frequency;
        Sections = sections;
    }
    public Guid Id { get; private set; }
    public Guid ScheduleId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public Period Period { get; private set; }
    public Guid SubjectId { get; private set; }
    public string Lecturer { get; private set; }
    public string Location { get; private set; }
    public SessionType SessionType { get; private set; }
    public SessionFrequency Frequency { get; private set; }
    public List<Section> Sections { get; private set; }
    
    public static Session Create(
        Guid scheduleId,
        DayOfWeek dayOfWeek,
        Period period,
        Guid subjectId,
        string lecturer,
        string location,
        SessionType sessionType,
        SessionFrequency frequency,
        List<Section> sections) => new(Guid.NewGuid(), scheduleId, dayOfWeek, period, subjectId, lecturer, location, sessionType, frequency, sections);
}