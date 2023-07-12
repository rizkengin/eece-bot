using EECEBOT.Application.Common.Persistence;
using EECEBOT.Domain.UserAggregate;
using EECEBOT.Domain.UserAggregate.Entities;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly IDocumentSession _documentSession;

    public UserRepository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _documentSession
            .Query<User>()
            .FirstOrDefaultAsync(x => string.Equals(x.Email, email, StringComparison.CurrentCultureIgnoreCase), cancellationToken);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _documentSession
            .Query<User>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task AddRefreshTokenAsync(User user, RefreshToken refreshToken, RefreshToken? oldRefreshToken = null,
        CancellationToken cancellationToken = default)
    {
        if (oldRefreshToken is not null)
        {
            user.RemoveRefreshToken(oldRefreshToken);
        }
        
        user.AddRefreshToken(refreshToken);
        
        _documentSession.Update(user);
        
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}