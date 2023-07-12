using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Domain.UserAggregate;
using EECEBOT.Domain.UserAggregate.Entities;
using ErrorOr;

namespace EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;

public interface IJwtTokenProvider
{
    string GenerateJwtToken(Guid userId, string email, string role);
    RefreshToken GenerateRefreshToken(string jwtToken);
    Task<ErrorOr<RefreshTokenResult>> RefreshJwtToken(string token, string refreshToken, CancellationToken cancellationToken = default);
    Task<ErrorOr<User>> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
}