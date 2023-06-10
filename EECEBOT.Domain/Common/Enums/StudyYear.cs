namespace EECEBOT.Domain.Common.Enums;

public enum StudyYear
{
    None,
    First,
    Second,
    Third,
    Fourth,
}

public static class StudyYearExtensions
{
    public static string ToFriendlyString(this StudyYear studyYear) => studyYear switch
    {
        StudyYear.None => "None",
        StudyYear.First => "1st",
        StudyYear.Second => "2nd",
        StudyYear.Third => "3rd",
        StudyYear.Fourth => "4th",
        _ => throw new ArgumentOutOfRangeException(nameof(studyYear), studyYear, null)
    };
}