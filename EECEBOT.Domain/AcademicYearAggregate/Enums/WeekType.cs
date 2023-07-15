namespace EECEBOT.Domain.AcademicYearAggregate.Enums;

public enum WeekType
{
    Even,
    Odd
}

public static class WeekTypeExtensions
{
    public static SessionFrequency ToSessionFrequency(this WeekType weekType)
    {
        return weekType switch
        {
            WeekType.Even => SessionFrequency.EvenWeeks,
            WeekType.Odd => SessionFrequency.OddWeeks,
            _ => throw new ArgumentOutOfRangeException(nameof(weekType), weekType, null)
        };
    }
}