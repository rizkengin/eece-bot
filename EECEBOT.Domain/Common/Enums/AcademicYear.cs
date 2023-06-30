namespace EECEBOT.Domain.Common.Enums;

public enum AcademicYear
{
    None,
    FirstYear,
    SecondYear,
    ThirdYear,
    FourthYear,
}

public static class AcademicYearExtensions
{
    public static string ToFriendlyString(this AcademicYear academicYear) => academicYear switch
    {
        AcademicYear.None => "None",
        AcademicYear.FirstYear => "1st Year",
        AcademicYear.SecondYear => "2nd Year",
        AcademicYear.ThirdYear => "3rd Year",
        AcademicYear.FourthYear => "4th Year",
        _ => throw new ArgumentOutOfRangeException(nameof(academicYear), academicYear, null)
    };
}