using ErrorOr;
using Microsoft.AspNetCore.Http;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class CommonErrors
    {
        public static Error TenantIdNotSet => Error.Custom(StatusCodes.Status500InternalServerError, "TenantIdNotSet",
            "The tenant id is not set.");
    }
}