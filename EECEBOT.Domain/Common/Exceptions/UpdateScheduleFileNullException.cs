namespace EECEBOT.Domain.Common.Exceptions;


public class UpdateScheduleFileNullException : Exception
{
    public UpdateScheduleFileNullException() 
        : base("The schedule file was not updated because the schedule is null.")
    {
    }
}