namespace EECEBOT.Contracts.Authentication;

public sealed record AuthenticationResponse(
    string Id,
    string Email,
    string Role,
    string Token,
    string RefreshToken,
    string ExpiresAt);