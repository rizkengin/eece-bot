using EECEBOT.Domain.Common.Enums;
using Newtonsoft.Json;

namespace EECEBOT.Domain.Link;

public class Link
{
    [JsonConstructor]
    private Link(Guid id,
        string name,
        string url,
        StudyYear studyYear)
    {
        Id = id;
        Name = name;
        Url = url;
        StudyYear = studyYear;
    }
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Url { get; private set; }
    public StudyYear StudyYear { get; private set; }

    public static Link Create(string name, string url, StudyYear studyYear) => new Link(Guid.NewGuid(), name, url, studyYear);
}