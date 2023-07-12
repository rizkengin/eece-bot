namespace EECEBOT.Application.Common.Persistence;

public interface IUnitOfWork
{
    void Update<T>(T entity) where T : class;
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}