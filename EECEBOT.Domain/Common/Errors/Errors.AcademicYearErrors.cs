using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class AcademicYearErrors
    {
        public static Error AcademicYearNotFound => Error.Custom((int)HttpStatusCode.NotFound, "AcademicYear.NotFound",
            "Academic year not found.");
    }
}