using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class LinksErrors
    {
        public static Error InvalidAcademicYear => Error.Custom(StatusCodes.Status400BadRequest, "InvalidAcademicYear", "The academic year is invalid.");
    }
}