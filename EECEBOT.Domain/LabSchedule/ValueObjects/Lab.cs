using EECEBOT.Domain.Schedule.Enums;

namespace EECEBOT.Domain.LabSchedule.ValueObjects;

public class Lab
{
    private Lab(
        string name,
        DateTime date,
        string location,
        Section section,
        Range? benchNumbersRange = null)
    {
        Name = name;
        Date = date;
        Location = location;
        BenchNumbersRange = benchNumbersRange;
        Section = section;
    }
    public string Name { get; private set; }
    public DateTime Date { get; private set; }
    public Section Section { get; private set; }
    public Range? BenchNumbersRange { get; private set; }
    public string Location { get; private set; }

    public static Lab Create(string name,
        DateTime date,
        string location,
        Section section,
        Range? benchNumbersRange = null) => new(name, date, location, section, benchNumbersRange);
}