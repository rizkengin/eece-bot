namespace EECEBOT.Domain.AcademicYearAggregate.Enums;

public enum Year
{
    None,
    FirstYear,
    SecondYear,
    ThirdYear,
    FourthYear,
}

public static class YearExtensions
{
    public static string ToFriendlyString(this Year year) => year switch
    {
        Year.None => "None",
        Year.FirstYear => "1st Year",
        Year.SecondYear => "2nd Year",
        Year.ThirdYear => "3rd Year",
        Year.FourthYear => "4th Year",
        _ => throw new ArgumentOutOfRangeException(nameof(year), year, null)
    };
}