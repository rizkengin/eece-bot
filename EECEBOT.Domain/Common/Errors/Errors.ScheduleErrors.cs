using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class ScheduleErrors
    {
        public static Error ScheduleNotFound => Error.Custom(StatusCodes.Status404NotFound, "ScheduleNotFound",
            "The schedule was not found.");
        
        public static Error ScheduleAlreadyExists => Error.Custom(StatusCodes.Status409Conflict, "ScheduleAlreadyExists",
            "The schedule already exists.");
        
        public static Error ScheduleStartDateMustBeSunday => Error.Custom(StatusCodes.Status400BadRequest, "ScheduleStartDateMustBeSunday",
            "The schedule start date must be a Sunday.");
    }
}