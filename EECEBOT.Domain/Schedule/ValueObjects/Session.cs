using EECEBOT.Domain.Schedule.Entities;
using EECEBOT.Domain.Schedule.Enums;

namespace EECEBOT.Domain.Schedule.ValueObjects;

public class Session
{
    private Session(DayOfWeek dayOfWeek,
        Period period,
        StudyGroup studyGroup,
        Subject subject,
        Lecturer lecturer,
        string location,
        SessionType sessionType,
        SessionTimeFrame timeFrame,
        Section? section)
    {
        DayOfWeek = dayOfWeek;
        Period = period;
        StudyGroup = studyGroup;
        Subject = subject;
        Lecturer = lecturer;
        Location = location;
        SessionType = sessionType;
        TimeFrame = timeFrame;
        Section = section;
    }
    public DayOfWeek DayOfWeek { get; private set; }
    public Period Period { get; private set; }
    public StudyGroup StudyGroup { get; private set; }
    public Subject Subject { get; private set; }
    public Lecturer Lecturer { get; private set; }
    public string Location { get; private set; }
    public SessionType SessionType { get; private set; }
    public SessionTimeFrame TimeFrame { get; private set; }
    public Section? Section { get; private set; }
    
    public static Session Create(DayOfWeek dayOfWeek,
        Period period,
        StudyGroup studyGroup,
        Subject subject,
        Lecturer lecturer,
        string location,
        SessionType sessionType,
        SessionTimeFrame timeFrame,
        Section? section = null) => new(dayOfWeek, period, studyGroup, subject, lecturer, location, sessionType, timeFrame, section);
}