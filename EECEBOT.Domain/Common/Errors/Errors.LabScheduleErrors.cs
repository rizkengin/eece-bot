using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class LabScheduleErrors
    {
        public static Error LabScheduleNotFound => Error.Custom((int)HttpStatusCode.NotFound, "LabSchedule.NotFound",
            "Lab schedule not found.");
        
        public static Error LabScheduleAlreadyExists => Error.Custom((int)HttpStatusCode.Conflict, "LabSchedule.AlreadyExists",
            "Lab schedule already exists.");

        public static Error LabScheduleSplitMethodIsByBenchNumberButBenchNumbersRangeIsInvalid => Error.Custom((int)HttpStatusCode.BadRequest,
            "LabSchedule.SplitMethodIsByBenchNumberButBenchNumbersRangeIsInvalid",
            "Lab schedule split method is by bench number but bench numbers range is invalid.");
    }
}