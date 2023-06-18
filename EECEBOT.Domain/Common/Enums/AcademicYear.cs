namespace EECEBOT.Domain.Common.Enums;

public enum AcademicYear
{
    None,
    First,
    Second,
    Third,
    Fourth,
}

public static class AcademicYearExtensions
{
    public static string ToFriendlyString(this AcademicYear academicYear) => academicYear switch
    {
        AcademicYear.None => "None",
        AcademicYear.First => "1st",
        AcademicYear.Second => "2nd",
        AcademicYear.Third => "3rd",
        AcademicYear.Fourth => "4th",
        _ => throw new ArgumentOutOfRangeException(nameof(academicYear), academicYear, null)
    };
}