using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;

namespace EECEBOT.Domain.AcademicYearAggregate.Entities;

public class Schedule
{
    private readonly List<Session> _sessions = new();
    private readonly List<Subject> _subjects = new();

    private Schedule(Guid id,
        DateOnly scheduleStartDate)
    {
        Id = id;
        ScheduleStartDate = scheduleStartDate;
    }
    
    public Guid Id { get; private set; }
    public DateOnly ScheduleStartDate { get; private set; }
    public Uri? FileUri { get; private set; }
    
    public IReadOnlyCollection<Session> Sessions
    {
        get => _sessions.ToArray();
        private set
        {
            _sessions.Clear();
            _sessions.AddRange(value);
        }
    }
    
    public IReadOnlyCollection<Subject> Subjects
    {
        get => _subjects.ToArray();
        private set
        {
            _subjects.Clear();
            _subjects.AddRange(value);
        }
    }
    public static ErrorOr<Schedule> TryCreate(DateOnly scheduleStartDate)
    {
        if (scheduleStartDate.DayOfWeek != DayOfWeek.Sunday)
            return Errors.ScheduleErrors.ScheduleStartDateMustBeSunday;

        return new Schedule(Guid.NewGuid(), scheduleStartDate);
    }
    
    internal void UpdateDateAndSessions(DateOnly scheduleStartDate, IEnumerable<Session> sessions)
    {
        ScheduleStartDate = scheduleStartDate;
        Sessions = sessions.ToList();
    }
    internal void AddSubject(Subject subject) => _subjects.Add(subject);
    internal void DeleteSubject(Subject subject) => _subjects.Remove(subject);
    internal void UpdateSubjects(IEnumerable<Subject> subjects) => Subjects = subjects.ToList();
    internal void UpdateFileUri(Uri fileUri) => FileUri = fileUri;

    internal void DeleteSubjectSessions(Guid subjectId)
    {
        var sessions = _sessions
            .Where(session => session.SubjectId == subjectId)
            .ToList();
        
        foreach (var session in sessions)
            _sessions.Remove(session);
    }
    
    public WeekType GetWeekType(DateOnly date)
    {
        var daysSpan = date.DayNumber - ScheduleStartDate.DayNumber;
        var weekNumber = daysSpan / 7;
        return weekNumber % 2 == 0 ? WeekType.Even : WeekType.Odd;
    }
}