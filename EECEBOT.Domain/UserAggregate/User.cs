using System.Globalization;
using EECEBOT.Domain.Common.Enums;
using EECEBOT.Domain.Common.Models;
using EECEBOT.Domain.UserAggregate.Entities;

namespace EECEBOT.Domain.UserAggregate;

public class User : AggregateRoot
{
    private readonly List<RefreshToken> _refreshTokens = new();
    
    private User(Guid id,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }

    public IReadOnlyCollection<RefreshToken> RefreshTokens
    {
        get => _refreshTokens.ToArray();
        private set
        {
            _refreshTokens.Clear();
            _refreshTokens.AddRange(value);
        }
    }
    
    public static User Create(string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        Role role) => new(Guid.NewGuid(),
        new CultureInfo("en-US", false).TextInfo.ToTitleCase(firstName.ToLower()),
        new CultureInfo("en-US", false).TextInfo.ToTitleCase(lastName.ToLower()),
        email.ToLower(),
        phoneNumber,
        password,
        role);
    
    public string FullName => $"{FirstName} {LastName}";
    
    public void RemoveRefreshToken(RefreshToken refreshToken) => _refreshTokens.Remove(refreshToken);

    public void AddRefreshToken(RefreshToken refreshToken) => _refreshTokens.Add(refreshToken);
    
    public void RefreshTokensCleanup()
    {
        var expiredTokens = _refreshTokens
            .Where(t => !t.IsActive)
            .ToList();
        
        foreach (var expiredToken in expiredTokens)
        {
            _refreshTokens.Remove(expiredToken);
        }
    }

    public void ResetAccess()
    {
        _refreshTokens.Clear();
    }
}