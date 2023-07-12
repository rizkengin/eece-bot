namespace EECEBOT.Contracts.Authentication;

public sealed record RefreshTokenRequest(
    string Token,
    string RefreshToken);