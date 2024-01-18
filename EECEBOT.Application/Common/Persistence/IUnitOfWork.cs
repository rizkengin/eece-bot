namespace EECEBOT.Application.Common.Persistence;

public interface IUnitOfWork
{
    Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}