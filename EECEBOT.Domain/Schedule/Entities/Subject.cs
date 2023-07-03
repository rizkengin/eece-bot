namespace EECEBOT.Domain.Schedule.Entities;

public class Subject
{
    private Subject(
        Guid id,
        Guid scheduleId,
        string name,
        string code)
    {
        Id = id;
        ScheduleId = scheduleId;
        Name = name;
        Code = code;
    }
    public Guid Id { get; private set; }
    public Guid ScheduleId { get; private set; }
    public string Name { get; private set; }
    public string Code { get; private set; }

    public static Subject Create(
        Guid scheduleId,
        string name,
        string code) => new(Guid.NewGuid(), scheduleId, name, code);
}