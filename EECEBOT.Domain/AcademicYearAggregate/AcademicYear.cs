using EECEBOT.Domain.AcademicYearAggregate.Entities;
using EECEBOT.Domain.AcademicYearAggregate.Enums;
using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.Common.Exceptions;
using EECEBOT.Domain.Common.Models;
using EECEBOT.Domain.DomainEvents;
using ErrorOr;

namespace EECEBOT.Domain.AcademicYearAggregate;

public class AcademicYear : AggregateRoot
{
    private readonly List<Link> _links = new();
    private readonly List<Exam> _exams = new();
    private readonly List<Deadline> _deadlines = new();

    private AcademicYear(
        Guid id,
        Year year)
    {
        Id = id;
        Year = year;
    }
    
    public Guid Id { get; private set; }
    public Year Year { get; private set; }
    public Schedule? Schedule { get; private set; }
    public LabSchedule? LabSchedule { get; private set; }

    public IReadOnlyCollection<Link> Links
    {
        get => _links.ToArray();
        private set
        {
            _links.Clear();
            _links.AddRange(value);
        }
    }
    
    public IReadOnlyCollection<Exam> Exams
    {
        get => _exams.ToArray();
        private set
        {
            _exams.Clear();
            _exams.AddRange(value);
        }
    }
    
    public IReadOnlyCollection<Deadline> Deadlines
    {
        get => _deadlines.ToArray();
        private set
        {
            _deadlines.Clear();
            _deadlines.AddRange(value);
        }
    }
    
    public static AcademicYear Create(Year year) => new(Guid.NewGuid(), year);

    public void SetSchedule(Schedule schedule) => Schedule = schedule;

    public void SetLabSchedule(LabSchedule labSchedule) => LabSchedule = labSchedule;

    public ErrorOr<Updated> TryUpdateSchedule(DateOnly scheduleStartDate, IEnumerable<Session> sessions, List<Guid> subjectsIds)
    {
        if (Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        if (scheduleStartDate.DayOfWeek != DayOfWeek.Sunday)
            return Errors.ScheduleErrors.ScheduleStartDateMustBeSunday;

        if (!subjectsIds.Distinct().All(x => Schedule.Subjects.Select(s => s.Id).Contains(x)))
            return Errors.ScheduleErrors.InvalidSubjectsIds;
        
        Schedule.UpdateDateAndSessions(scheduleStartDate, sessions);
        
        RaiseDomainEvent(new ScheduleUpdatedDomainEvent(Year));

        return new Updated();
    }
    
    public ErrorOr<Updated> TryUpdateLabScheduleLabs(List<Lab> labs)
    {
        if (LabSchedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        if (labs.Exists(x => x.BenchNumbersRange.End.Value <= x.BenchNumbersRange.Start.Value))
            return Errors.LabScheduleErrors.LabScheduleSplitMethodIsByBenchNumberButBenchNumbersRangeIsInvalid;
        
        LabSchedule.UpdateLabs(labs);
        
        RaiseDomainEvent(new LabScheduleUpdatedDomainEvent(Year));

        return new Updated();
    }
    
    public ErrorOr<Created> TryAddScheduleSubject(Subject subject)
    {
        if (Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        if(Schedule.Subjects.Any(x => 
               string.Equals(x.Name, subject.Name, StringComparison.InvariantCultureIgnoreCase) ||
               string.Equals(x.Code, subject.Code, StringComparison.InvariantCultureIgnoreCase)))
            return Errors.ScheduleErrors.SubjectAlreadyExists;

        Schedule.AddSubject(subject);

        return new Created();
    }
    
    public ErrorOr<Deleted> TryDeleteScheduleSubject(Guid subjectId)
    {
        if (Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;

        var subject = Schedule.Subjects.FirstOrDefault(x => x.Id == subjectId);
        
        if(subject is null)
            return Errors.ScheduleErrors.SubjectNotFound;
        
        Schedule.DeleteSubjectSessions(subjectId);
        
        Schedule.DeleteSubject(subject);

        return new Deleted();
    }

    public ErrorOr<Updated> TryUpdateScheduleSubject(Guid subjectId, string name, string code)
    {
        if (Schedule is null)
            return Errors.ScheduleErrors.ScheduleNotFound;
        
        var subject = Schedule.Subjects.FirstOrDefault(x => x.Id == subjectId);
        
        if(subject is null)
            return Errors.ScheduleErrors.SubjectNotFound;
        
        Schedule.UpdateSubject(subject, name, code);

        return new Updated();
    }

    public void UpdateScheduleFileUri(Uri fileUri)
    {
        if (Schedule is null)
            throw new UpdateScheduleFileNullException();

        Schedule.UpdateFileUri(fileUri);
    }
    
    public void UpdateLabScheduleFileUri(Uri fileUri)
    {
        if (LabSchedule is null)
            throw new UpdateLabScheduleFileNullException();
        
        LabSchedule.UpdateFileUri(fileUri);
    }

    public void UpdateLinks(IEnumerable<Link> links)
    {
        Links = links.ToList();
        RaiseDomainEvent(new LinksUpdatedDomainEvent(Year));
    }

    public void UpdateExams(IEnumerable<Exam> exams)
    {
        Exams = exams.ToList();
        RaiseDomainEvent(new ExamsUpdatedDomainEvent(Year));
    }

    public void UpdateDeadlines(IEnumerable<Deadline> deadlines)
    {
        Deadlines = deadlines.ToList();
        RaiseDomainEvent(new DeadlinesUpdatedDomainEvent(Year));
    }
}