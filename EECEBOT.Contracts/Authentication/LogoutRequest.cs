namespace EECEBOT.Contracts.Authentication;

public sealed record LogoutRequest(
    string Token,
    string RefreshToken);