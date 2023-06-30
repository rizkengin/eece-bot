namespace EECEBOT.Domain.Schedule.Entities;

public class Subject
{
    private Subject(string name,
        string code)
    {
        Name = name;
        Code = code;
    }
    public string Name { get; private set; }
    public string Code { get; private set; }

    public static Subject Create(string name,
        string code) => new(name, code);
}