using EECEBOT.Domain.Common.Enums;
using Newtonsoft.Json;

namespace EECEBOT.Domain.TelegramUser;

public class TelegramUser
{
    [JsonConstructor]
    private TelegramUser(Guid id,
                         string firstName,
                         string? lastName,
                         string? username,
                         long chatId,
                         StudyYear studyYear,
                         CurrentState currentState)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        ChatId = chatId;
        StudyYear = studyYear;
        CurrentState = currentState;
    }
    
    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Username { get; private set; }
    public long ChatId { get; private set; }
    public StudyYear StudyYear { get; private set; }
    public CurrentState CurrentState { get; private set; }
    
    public static TelegramUser Create(string firstName,
                                      string? lastName,
                                      string? username,
                                      long chatId) => new TelegramUser(Guid.NewGuid(), firstName, lastName, username, chatId, StudyYear.None, CurrentState.PickingStudyYear);
    
    public void UpdateStudyYear(StudyYear studyYear)
    {
        StudyYear = studyYear;
        CurrentState = CurrentState.None;
    }
}