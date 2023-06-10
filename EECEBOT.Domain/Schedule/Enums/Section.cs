namespace EECEBOT.Domain.Schedule.Enums;

public enum Section
{
    Section1,
    Section2,
    Section3,
    Section4
}

public static class SectionExtensions
{
    public static string ToFriendlyString(this Section section)
    {
        return section switch
        {
            Section.Section1 => "Section 1",
            Section.Section2 => "Section 2",
            Section.Section3 => "Section 3",
            Section.Section4 => "Section 4",
            _ => throw new ArgumentOutOfRangeException(nameof(section), section, null)
        };
    }
}