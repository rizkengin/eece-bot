namespace EECEBOT.Domain.AcademicYearAggregate.Entities;

public class Link 
{
    private Link(Guid id,
        string name,
        Uri url)
    {
        Id = id;
        Name = name;
        Url = url;
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Uri Url { get; private set; }

    public static Link Create(string name, Uri url) 
        => new(Guid.NewGuid(), name, url);
}