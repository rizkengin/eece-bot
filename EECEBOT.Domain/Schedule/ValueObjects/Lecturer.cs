namespace EECEBOT.Domain.Schedule.ValueObjects;

public class Lecturer
{
    private Lecturer(string name)
    {
        Name = name;
    }
    public string Name { get; private set; }
    
    public static Lecturer Create(string name) => new(name);
}