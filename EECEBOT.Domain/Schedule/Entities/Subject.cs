using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Domain.Schedule.Entities;

public class Subject
{
    private Subject(Guid id,
        string name,
        string code,
        AcademicYear academicYear)
    {
        Id = id;
        Name = name;
        Code = code;
        AcademicYear = academicYear;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    
    public static Subject Create(string name,
        string code,
        AcademicYear academicYear) => new(Guid.NewGuid(), name, code, academicYear);
}