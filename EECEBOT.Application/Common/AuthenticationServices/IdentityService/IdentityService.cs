using System.Globalization;
using EECEBOT.Application.Authentication.ResultModels;
using EECEBOT.Application.Common.AuthenticationServices.JwtTokenProvider;
using EECEBOT.Application.Common.AuthenticationServices.PasswordHasher;
using EECEBOT.Application.Common.Persistence;
using EECEBOT.Application.Common.Services;
using EECEBOT.Domain.Common.Errors;
using ErrorOr;

namespace EECEBOT.Application.Common.AuthenticationServices.IdentityService;

public class IdentityService : IIdentityService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenProvider _jwtTokenProvider;

    public IdentityService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenProvider jwtTokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenProvider = jwtTokenProvider;
    }

    public async Task<ErrorOr<AuthenticationResult>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        
        if (user is null)
        {
            return Errors.AuthenticationErrors.InvalidCredentials;
        }
        
        if (!_passwordHasher.VerifyPassword(password, user.Password))
        {
            return Errors.AuthenticationErrors.InvalidCredentials;
        }
        
        var token = _jwtTokenProvider.GenerateJwtToken(
            user.Id,
            user.Email,
            user.Role.ToString());
        
        var refreshToken = _jwtTokenProvider.GenerateRefreshToken(token);
        
        await _userRepository.AddRefreshTokenAsync(user, refreshToken, cancellationToken: cancellationToken);
        
        return new AuthenticationResult(
            user.Id.ToString(),
            user.Email,
            user.Role.ToString(),
            token,
            refreshToken.Token,
            refreshToken.ExpiresOn.ToString(CultureInfo.InvariantCulture));
    }
}