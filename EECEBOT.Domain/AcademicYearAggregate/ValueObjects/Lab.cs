using EECEBOT.Domain.AcademicYearAggregate.Enums;

namespace EECEBOT.Domain.AcademicYearAggregate.ValueObjects;

public class Lab
{
    private Lab(
        string name,
        DateTimeOffset date,
        string location,
        Section section,
        Range benchNumbersRange)
    {
        Name = name;
        Date = date;
        Location = location;
        BenchNumbersRange = benchNumbersRange;
        Section = section;
    }
    public string Name { get; private set; }
    public DateTimeOffset Date { get; private set; }
    public Section Section { get; private set; }
    public Range BenchNumbersRange { get; private set; }
    public string Location { get; private set; }

    public static Lab Create(string name,
        DateTimeOffset date,
        string location,
        Section section,
        Range benchNumbersRange) => new(name, date, location, section, benchNumbersRange);
}