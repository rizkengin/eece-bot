namespace EECEBOT.Application.Authentication.ResultModels;

public record RefreshTokenResult(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt);