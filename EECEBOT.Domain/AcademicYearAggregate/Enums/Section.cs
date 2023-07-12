namespace EECEBOT.Domain.AcademicYearAggregate.Enums;

public enum Section
{
    SectionOne,
    SectionTwo,
    SectionThree,
    SectionFour
}

public static class SectionExtensions
{
    public static string ToFriendlyString(this Section section)
    {
        return section switch
        {
            Section.SectionOne => "Section 1",
            Section.SectionTwo => "Section 2",
            Section.SectionThree => "Section 3",
            Section.SectionFour => "Section 4",
            _ => throw new ArgumentOutOfRangeException(nameof(section), section, null)
        };
    }
}