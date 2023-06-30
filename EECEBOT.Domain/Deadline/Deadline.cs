using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Domain.Deadline;

public class Deadline
{
    private Deadline(Guid id,
        string title,
        string description,
        DateTimeOffset dueDate,
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
    public DateTimeOffset DueDate { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    
    public static Deadline Create(string title, string description, DateTimeOffset dueDate, AcademicYear academicYear) 
        => new(Guid.NewGuid(), title, description, dueDate, academicYear);
    
    public TimeSpan GetTimeLeft() =>  DueDate - DateTimeOffset.UtcNow;
}