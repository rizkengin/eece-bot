using EECEBOT.Domain.Common.Enums;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Exam;

public class Exam
{
    [JsonConstructor]
    private Exam(Guid id,
        string name,
        ExamType type,
        string description,
        DateTime date,
        AcademicYear academicYear,
        string? location)
    {
        Id = id;
        Name = name;
        Type = type;
        Description = description;
        Location = location;
        Date = date;
        AcademicYear = academicYear;
    }
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public ExamType Type { get; private set; }
    public string Description { get; private set; }
    public string? Location { get; private set; }
    public DateTime Date { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    
    public static Exam Create(string name,
        ExamType type,
        string description,
        DateTime date,
        AcademicYear academicYear,
        string? location = null) => new Exam(Guid.NewGuid(), name, type, description, date, academicYear, location);
}