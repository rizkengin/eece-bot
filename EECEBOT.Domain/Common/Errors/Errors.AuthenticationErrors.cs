using System.Net;
using ErrorOr;

namespace EECEBOT.Domain.Common.Errors;

public static partial class Errors
{
    public static class AuthenticationErrors
    {
        public static Error InvalidCredentials => Error.Custom((int)HttpStatusCode.Unauthorized,
            "Authentication.InvalidCredentials",
            "Invalid credentials.");
        
        public static Error InvalidToken => Error.Custom((int)HttpStatusCode.BadRequest,
            "Authentication.InvalidToken",
            "Invalid token.");
        
        public static Error TokenExpired => Error.Custom((int)HttpStatusCode.BadRequest,
            "Authentication.TokenExpired",
            "Token expired.");
        
        public static Error RefreshTokenExpired => Error.Custom((int)HttpStatusCode.BadRequest,
            "Authentication.RefreshTokenExpired",
            "Refresh token expired.");
    }
}