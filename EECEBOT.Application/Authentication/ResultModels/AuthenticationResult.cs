namespace EECEBOT.Application.Authentication.ResultModels;

public sealed record AuthenticationResult(
    string Id,
    string Email,
    string Role,
    string Token,
    string RefreshToken,
    string ExpiresAt);