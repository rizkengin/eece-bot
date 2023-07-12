using System.Runtime.Serialization;

namespace EECEBOT.Domain.Common.Exceptions;

[Serializable]
public class UpdateScheduleFileNullException : Exception
{
    public UpdateScheduleFileNullException() 
        : base("The schedule file was not updated because the schedule is null.")
    {
    }
    
    protected UpdateScheduleFileNullException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}