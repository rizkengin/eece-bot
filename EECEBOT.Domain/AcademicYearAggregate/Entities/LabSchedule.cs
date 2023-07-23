using EECEBOT.Domain.AcademicYearAggregate.ValueObjects;

namespace EECEBOT.Domain.AcademicYearAggregate.Entities;

public class LabSchedule
{
    private readonly List<Lab> _labs = new();
    
    private LabSchedule(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; private set; }
    public Uri? FileUri { get; private set; }

    public IReadOnlyCollection<Lab> Labs
    {
        get => _labs.ToArray();
        private set
        {
            _labs.Clear();
            _labs.AddRange(value);
        }
    }

    public static LabSchedule Create() => new(Guid.NewGuid());
    internal void UpdateLabs(IEnumerable<Lab> labs) => Labs = labs.ToList();
    internal void UpdateFileUri(Uri fileUri) => FileUri = fileUri;
}