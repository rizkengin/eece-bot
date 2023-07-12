namespace EECEBOT.Contracts.Authentication;

public sealed record RefreshTokenResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt);