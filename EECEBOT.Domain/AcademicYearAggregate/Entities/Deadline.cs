namespace EECEBOT.Domain.AcademicYearAggregate.Entities;

public class Deadline 
{
    private Deadline(Guid id,
        string title,
        string description,
        DateTimeOffset dueDate)
    {
        Id = id;
        Title = title;
        Description = description;
        DueDate = dueDate;
    }
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset DueDate { get; private set; }

    public static Deadline Create(string title, string description, DateTimeOffset dueDate) 
        => new(Guid.NewGuid(), title, description, dueDate);
    
    public TimeSpan GetTimeLeft() =>  DueDate - DateTimeOffset.UtcNow;
}