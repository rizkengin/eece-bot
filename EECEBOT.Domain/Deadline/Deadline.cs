using EECEBOT.Domain.Common.Enums;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Deadline;

public class Deadline
{
    [JsonConstructor]
    public Deadline(Guid id,
        string title,
        string description,
        DateTime dueDate,
        StudyYear studyYear)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        StudyYear = studyYear;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public StudyYear StudyYear { get; private set; }
    
    public static Deadline Create(string title, string description, DateTime dueDate, StudyYear studyYear) => 
        new Deadline(Guid.NewGuid(), title, description, dueDate, studyYear);
    
    public TimeSpan GetTimeLeft() => DueDate - DateTime.UtcNow;
}