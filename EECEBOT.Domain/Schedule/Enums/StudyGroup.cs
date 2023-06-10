namespace EECEBOT.Domain.Schedule.Enums;

public enum StudyGroup
{
    GroupA,
    GroupB
}

public static class StudyGroupExtensions
{
    public static string ToFriendlyString(this StudyGroup studyGroup) =>
        studyGroup switch
        {
            StudyGroup.GroupA => "Group A",
            StudyGroup.GroupB => "Group B",
            _ => throw new ArgumentOutOfRangeException(nameof(studyGroup), studyGroup, null)
        };
}