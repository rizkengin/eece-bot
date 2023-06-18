namespace EECEBOT.Domain.Schedule.Enums;

public enum SessionFrequency
{
    Always,
    OddWeeks,
    EvenWeeks
}

public static class SessionFrequencyExtensions
{
    public static string ToFriendlyString(this SessionFrequency sessionFrequency) => sessionFrequency switch
    {
        SessionFrequency.Always => "Always",
        SessionFrequency.OddWeeks => "Odd Weeks",
        SessionFrequency.EvenWeeks => "Even Weeks",
        _ => throw new ArgumentOutOfRangeException(nameof(sessionFrequency), sessionFrequency, null),
    };
}