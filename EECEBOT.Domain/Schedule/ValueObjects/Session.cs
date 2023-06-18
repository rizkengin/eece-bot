using EECEBOT.Domain.Schedule.Entities;
using EECEBOT.Domain.Schedule.Enums;

namespace EECEBOT.Domain.Schedule.ValueObjects;

public class Session
{
    private Session(DayOfWeek dayOfWeek,
        Period period,
        Subject subject,
        Lecturer lecturer,
        string location,
        SessionType sessionType,
        SessionFrequency frequency,
        List<Section> sections)
    {
        DayOfWeek = dayOfWeek;
        Period = period;
        Subject = subject;
        Lecturer = lecturer;
        Location = location;
        SessionType = sessionType;
        Frequency = frequency;
        Sections = sections;
    }
    public DayOfWeek DayOfWeek { get; private set; }
    public Period Period { get; private set; }
    public Subject Subject { get; private set; }
    public Lecturer Lecturer { get; private set; }
    public string Location { get; private set; }
    public SessionType SessionType { get; private set; }
    public SessionFrequency Frequency { get; private set; }
    public List<Section> Sections { get; private set; }
    
    public static Session Create(DayOfWeek dayOfWeek,
        Period period,
        Subject subject,
        Lecturer lecturer,
        string location,
        SessionType sessionType,
        SessionFrequency frequency,
        List<Section> sections) => new(dayOfWeek, period, subject, lecturer, location, sessionType, frequency, sections);
}