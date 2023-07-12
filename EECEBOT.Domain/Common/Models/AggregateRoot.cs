using EECEBOT.Domain.Common.Interfaces;

namespace EECEBOT.Domain.Common.Models;

public abstract class AggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();
    public void ClearDomainEvents() => _domainEvents.Clear();
}