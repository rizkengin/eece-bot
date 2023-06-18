using EECEBOT.Domain.Common.Enums;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Link;

public class Link
{
    [JsonConstructor]
    private Link(Guid id,
        string name,
        string url,
        AcademicYear academicYear)
    {
        Id = id;
        Name = name;
        Url = url;
        AcademicYear = academicYear;
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Url { get; private set; }
    public AcademicYear AcademicYear { get; private set; }

    public static Link Create(string name, string url, AcademicYear academicYear) => new Link(Guid.NewGuid(), name, url, academicYear);
}