namespace EECEBOT.Domain.Common.Enums;

public enum CurrentState
{
    None,
    PickingAcademicYear,
    PickingSection,
    PickingBenchNumber
}

public static class CurrentStateExtensions
{
    public static string ToFriendlyString(this CurrentState currentState)
    {
        return currentState switch
        {
            CurrentState.None => "None",
            CurrentState.PickingAcademicYear => "Picking academic year.",
            CurrentState.PickingSection => "Picking section.",
            CurrentState.PickingBenchNumber => "Picking bench number.",
            _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
        };
    }
}