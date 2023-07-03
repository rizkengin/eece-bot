using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class LabScheduleErrors
    {
        public static Error LabScheduleAlreadyExists => Error.Custom((int)HttpStatusCode.Conflict,
            "LabScheduleAlreadyExists", "A lab schedule already exists for the given academic year.");
        
        public static Error LabScheduleNotFound => Error.Custom((int)HttpStatusCode.NotFound,
            "LabScheduleNotFound", "Lab schedule not found.");

        public static Error LabScheduleSplitMethodBySectionButBenchNumbersRangeEndIsGreaterThanZero => Error.Custom(
            (int)HttpStatusCode.BadRequest,
            "LabScheduleSplitMethodBySectionButBenchNumbersRangeEndIsGreaterThanZero",
            "Lab schedule split method is by section but bench numbers range end is greater than zero.");
        
        public static Error LabScheduleSplitMethodByBenchNumberButBenchNumbersRangeEndIsZero => Error.Custom(
            (int)HttpStatusCode.BadRequest,
            "LabScheduleSplitMethodByBenchNumberButBenchNumbersRangeEndIsZero",
            "Lab schedule split method is by bench number but bench numbers range end is zero.");
    }
}