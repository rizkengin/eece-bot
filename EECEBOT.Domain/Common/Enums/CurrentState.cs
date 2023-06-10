namespace EECEBOT.Domain.Common.Enums;

public enum CurrentState
{
    None,
    PickingStudyYear,
}

public static class CurrentStateExtensions
{
    public static string ToFriendlyString(this CurrentState currentState)
    {
        return currentState switch
        {
            CurrentState.None => "None",
            CurrentState.PickingStudyYear => "Picking study year",
            _ => throw new ArgumentOutOfRangeException(nameof(currentState), currentState, null)
        };
    }
}