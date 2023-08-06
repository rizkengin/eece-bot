using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class LabScheduleErrors
    {
        public static Error BenchNumbersRangeIsInvalid => Error.Custom((int)HttpStatusCode.BadRequest,
            "LabSchedule.BenchNumbersRangeIsInvalid",
            "Bench numbers range is invalid.");
    }
}