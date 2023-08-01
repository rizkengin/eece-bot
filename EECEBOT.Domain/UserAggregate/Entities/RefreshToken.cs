namespace EECEBOT.Domain.UserAggregate.Entities;

public class RefreshToken
{
    private RefreshToken(
        Guid id,
        string token,
        string jwtId,
        bool isUsed,
        bool isInvalidated,
        DateTimeOffset expiresOn)
    {
        Id = id;
        Token = token;
        JwtId = jwtId;
        IsUsed = isUsed;
        IsInvalidated = isInvalidated;
        ExpiresOn = expiresOn;
    }

    public static RefreshToken Create(
        string token,
        string jwtId,
        DateTimeOffset expiresOn)
    {
        return new RefreshToken(Guid.NewGuid(), token, jwtId, false, false, expiresOn);
    }
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public string JwtId { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsInvalidated { get; private set; }
    public DateTimeOffset ExpiresOn { get; private set; }
    public DateTimeOffset? RevokedOn { get; private set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresOn;
    public bool IsActive => !IsExpired && !IsUsed && !IsInvalidated;
    public void Expire()
    {
        IsUsed = true;
        RevokedOn = DateTimeOffset.UtcNow;
    }

    public void Invalidate()
    {
        IsInvalidated = true;
        RevokedOn = DateTimeOffset.UtcNow;
    }
}