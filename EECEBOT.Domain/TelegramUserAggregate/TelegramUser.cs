using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Models;

namespace EECEBOT.Domain.TelegramUserAggregate;

public class TelegramUser : AggregateRoot
{
    private TelegramUser(Guid id,
                         long telegramId,
                         string firstName,
                         string? lastName,
                         string? username,
                         long chatId,
                         Year year,
                         CurrentState currentState,
                         Section? section = null,
                         int? benchNumber = null)
    {
        Id = id;
        TelegramId = telegramId;
        FirstName = firstName;
        LastName = lastName;
        Username = username;
        ChatId = chatId;
        Year = year;
        Section = section;
        BenchNumber = benchNumber;
        CurrentState = currentState;
    }
    
    public Guid Id { get; private set; }
    public long TelegramId { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? Username { get; private set; }
    public long ChatId { get; private set; }
    public Year Year { get; private set; }
    public Section? Section { get; private set; }
    public int? BenchNumber { get; private set; }
    public CurrentState CurrentState { get; private set; }
    
    public static TelegramUser Create(string firstName,
                                      long telegramId,
                                      long chatId,
                                      string? lastName = null,
                                      string? username = null) 
        => new TelegramUser(Guid.NewGuid(),
            telegramId,
            firstName,
            lastName,
            username,
            chatId,
            Year.None,
            CurrentState.PickingAcademicYear);
    
    public void UpdateAcademicYear(Year year)
    {
        Year = year;
        CurrentState = CurrentState.PickingSection;
    }
    
    public void UpdateSection(Section section)
    {
        Section = section;
        CurrentState = CurrentState.PickingBenchNumber;
    }
    
    public void UpdateBenchNumber(int benchNumber)
    {
        BenchNumber = benchNumber;
        CurrentState = CurrentState.None;
    }

    public void ResetAcademicYear()
    {
        Year = Year.None;
        Section = null;
        BenchNumber = null;
        CurrentState = CurrentState.PickingAcademicYear;
    }
}