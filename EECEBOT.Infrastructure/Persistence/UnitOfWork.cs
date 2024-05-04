using EECEBOT.Application.Common.Persistence;
using Marten;

namespace EECEBOT.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _documentSession;

    public UnitOfWork(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
    {
        _documentSession.Update(entity);
        
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}