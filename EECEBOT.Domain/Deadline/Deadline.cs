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
        AcademicYear academicYear)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
        AcademicYear = academicYear;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    
    public static Deadline Create(string title, string description, DateTime dueDate, AcademicYear academicYear) => 
        new Deadline(Guid.NewGuid(), title, description, dueDate, academicYear);
    
    public TimeSpan GetTimeLeft() => DueDate - DateTime.UtcNow;
}