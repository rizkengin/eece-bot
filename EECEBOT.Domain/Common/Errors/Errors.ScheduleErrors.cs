using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class ScheduleErrors
    {
        public static Error ScheduleNotFound => Error.Custom((int)HttpStatusCode.NotFound, "Schedule.NotFound",
            "Schedule not found.");
        
        public static Error ScheduleStartDateMustBeSunday => Error.Custom((int)HttpStatusCode.BadRequest, "Schedule.StartDateMustBeSunday",
            "Schedule start date must be Sunday.");
        
        public static Error ScheduleAlreadyExists => Error.Custom((int)HttpStatusCode.BadRequest, "Schedule.AlreadyExists",
            "Schedule already exists.");
        
        public static Error SubjectAlreadyExists => Error.Custom((int)HttpStatusCode.Conflict, "Subject.AlreadyExists",
            "Subject already exists.");
        
        public static Error SubjectNotFound => Error.Custom((int)HttpStatusCode.NotFound, "Subject.NotFound",
            "Subject not found.");
        
        public static Error InvalidSubjectsIds => Error.Custom((int)HttpStatusCode.BadRequest, "Subject.InvalidSubjectsIds",
            "Invalid subjects ids.");
    }
}