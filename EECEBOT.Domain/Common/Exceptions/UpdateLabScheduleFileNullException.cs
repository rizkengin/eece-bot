namespace EECEBOT.Domain.Common.Exceptions;


public class UpdateLabScheduleFileNullException : Exception
{
    public UpdateLabScheduleFileNullException() 
        : base("The lab schedule file was not updated because the lab schedule is null.")
    {
    }
}