namespace EECEBOT.API.Common.Exceptions;


public class InvalidApplicationTimeZoneIdException : Exception
{
    public InvalidApplicationTimeZoneIdException(string timeZoneId)
    : base($"The application time zone Id is invalid : {timeZoneId}")
    {
        TimeZoneId = timeZoneId;
    }
    
    public string? TimeZoneId { get; }
}