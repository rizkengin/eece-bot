using EECEBOT.Domain.UserAggregate;
using EECEBOT.Domain.UserAggregate.Entities;

namespace EECEBOT.Application.Common.Persistence;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRefreshTokenAsync(User user, RefreshToken refreshToken, RefreshToken? oldRefreshToken = null, CancellationToken cancellationToken = default);
}