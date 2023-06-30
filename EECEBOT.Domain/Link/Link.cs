using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Domain.Link;

public class Link
{
    private Link(Guid id,
        string name,
        Uri url,
        AcademicYear academicYear)
    {
        Id = id;
        Name = name;
        Url = url;
        AcademicYear = academicYear;
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Uri Url { get; private set; }
    public AcademicYear AcademicYear { get; private set; }

    public static Link Create(string name, Uri url, AcademicYear academicYear) => new Link(Guid.NewGuid(), name, url, academicYear);
}