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

    public void Update<T>(T entity) where T : class
    {
        _documentSession.Update(entity);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _documentSession.SaveChangesAsync(cancellationToken);
    }
}