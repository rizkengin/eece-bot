using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.Common.Errors;
using EECEBOT.Domain.UserAggregate;
using EECEBOT.Domain.UserAggregate.Entities;
using ErrorOr;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;

public class JwtTokenProvider : IJwtTokenProvider
{
    private readonly ITimeService _timeService;
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;

    public JwtTokenProvider(
        ITimeService timeService,
        IOptions<JwtSettings> jwtSettings,
        IUserRepository userRepository)
    {
        _timeService = timeService;
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateJwtToken(Guid userId, string email, string role)
    {
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTimeOffset.UtcNow.DateTime.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(securityToken);
    }

    public RefreshToken GenerateRefreshToken(string jwtToken)
    {
        var token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

        var tokenString = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var refreshToken = RefreshToken.Create(tokenString, token.Id, DateTimeOffset.UtcNow.AddDays(7));

        return refreshToken;
    }

    public async Task<ErrorOr<RefreshTokenResult>> RefreshJwtToken(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch (Exception)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (jwtToken is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (userId is null || !Guid.TryParse(userId, out var userGuid))
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var user = await _userRepository.GetByIdAsync(userGuid, cancellationToken);

        if (user is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (userRefreshToken is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (userRefreshToken.JwtId != jwtToken.Id)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (!userRefreshToken.IsActive)
        {
            return Errors.AuthenticationErrors.RefreshTokenExpired;
        }

        var newJwtToken = GenerateJwtToken(
            user.Id, 
            user.Email,
            user.Role.ToString());

        var newRefreshToken = GenerateRefreshToken(newJwtToken);

        await _userRepository.AddRefreshTokenAsync(user, newRefreshToken, userRefreshToken, cancellationToken);
        
        return new RefreshTokenResult(newJwtToken, newRefreshToken.Token, newRefreshToken.ExpiresOn.DateTime);
    }

    public async Task<ErrorOr<User>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
        catch (Exception)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (jwtToken is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        var tokenExpiry = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

        if (userId is null || !Guid.TryParse(userId, out var userIdGuid))
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (tokenExpiry is null || !long.TryParse(tokenExpiry, out var tokenExpiryLong))
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var tokenExpiryDate = DateTimeOffset.FromUnixTimeSeconds(tokenExpiryLong).UtcDateTime;

        if (tokenExpiryDate < _timeService.GetCurrentUtcTime())
        {
            return Errors.AuthenticationErrors.TokenExpired;
        }

        var user = await _userRepository.GetByIdAsync(userIdGuid, cancellationToken);

        if (user is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        var userRefreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == refreshToken);

        if (userRefreshToken is null)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        if (userRefreshToken.JwtId != jwtToken.Id)
        {
            return Errors.AuthenticationErrors.InvalidToken;
        }

        userRefreshToken.Invalidate();

        return user;
    }
}