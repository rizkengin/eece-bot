using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.LabSchedule.Common;
using EECEBOT.Domain.LabSchedule.ValueObjects;

namespace EECEBOT.Domain.LabSchedule;

public class LabSchedule
{
    private readonly List<Lab> _labs = new();
    
    private LabSchedule(Guid id,
        AcademicYear academicYear,
        SplitMethod splitMethod)
    {
        Id = id;
        AcademicYear = academicYear;
        SplitMethod = splitMethod;
    }
    public Guid Id { get; private set; }
    public AcademicYear AcademicYear { get; private set; }
    public SplitMethod SplitMethod { get; private set; }
    public Uri? FileUri { get; private set; }
    
    public IReadOnlyCollection<Lab> Labs => _labs.AsReadOnly();

    public static LabSchedule Create(
        AcademicYear academicYear,
        SplitMethod splitMethod) => new(Guid.NewGuid(), academicYear, splitMethod);
    
    public void UpdateLabs(IEnumerable<Lab> labs)
    {
        _labs.Clear();
        _labs.AddRange(labs);
    }
    
    public void UpdateFileUri(Uri fileUri) => FileUri = fileUri;
}