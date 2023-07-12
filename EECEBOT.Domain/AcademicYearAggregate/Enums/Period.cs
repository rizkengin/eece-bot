namespace EECEBOT.Domain.AcademicYearAggregate.Enums;

public enum Period
{
    FirstPeriod,
    SecondPeriod,
    ThirdPeriod,
    FourthPeriod,
    FifthPeriod,
    SixthPeriod,
}

public static class PeriodExtensions
{
    public static string ToFriendlyString(this Period period) => period switch
    {
        Period.FirstPeriod => "1st Period",
        Period.SecondPeriod => "2nd Period",
        Period.ThirdPeriod => "3rd Period",
        Period.FourthPeriod => "4th Period",
        Period.FifthPeriod => "5th Period",
        Period.SixthPeriod => "6th Period",
        _ => throw new ArgumentOutOfRangeException(nameof(period), period, null),
    };
}