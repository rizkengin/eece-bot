using System.Runtime.Serialization;

namespace EECEBOT.Domain.Common.Exceptions;

[Serializable]
public class UpdateLabScheduleFileNullException : Exception
{
    public UpdateLabScheduleFileNullException() 
        : base("The lab schedule file was not updated because the lab schedule is null.")
    {
    }
    
    protected UpdateLabScheduleFileNullException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}