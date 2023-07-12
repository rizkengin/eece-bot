namespace EECEBOT.Domain.AcademicYearAggregate.Entities;

public class Subject
{
    private Subject(
        Guid id,
        string name,
        string code)
    {
        Id = id;
        Name = name;
        Code = code;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }

    public static Subject Create(
        string name,
        string code) => new(Guid.NewGuid(), name, code);
}