using System.Runtime.Serialization;

namespace EECEBOT.API.Common.Exceptions;

[Serializable]
public class InvalidApplicationTimeZoneIdException : Exception
{
    public InvalidApplicationTimeZoneIdException(string timeZoneId)
    : base($"The application time zone Id is invalid : {timeZoneId}")
    {
        TimeZoneId = timeZoneId;
    }

    protected InvalidApplicationTimeZoneIdException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        TimeZoneId = info.GetString(nameof(TimeZoneId));
    }
    
    public string? TimeZoneId { get; }
}