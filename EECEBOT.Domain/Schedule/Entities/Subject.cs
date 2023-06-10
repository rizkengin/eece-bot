using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Domain.Schedule.Entities;

public class Subject
{
    private Subject(Guid id,
        string name,
        string code,
        StudyYear studyYear)
    {
        Id = id;
        Name = name;
        Code = code;
        StudyYear = studyYear;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public StudyYear StudyYear { get; private set; }
    
    public static Subject Create(string name,
        string code,
        StudyYear studyYear) => new(Guid.NewGuid(), name, code, studyYear);
}