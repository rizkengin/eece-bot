namespace EECEBOT.Domain.LabSchedule.Common;

public enum SplitMethod
{
    BySection,
    ByBenchNumber
}

public static class SplitMethodExtensions
{
    public static string ToFriendlyString(this SplitMethod splitMethod) =>
        splitMethod switch
        {
            SplitMethod.BySection => "By Section",
            SplitMethod.ByBenchNumber => "By Bench Number",
            _ => throw new ArgumentOutOfRangeException(nameof(splitMethod), splitMethod, null)
        };
}