using EECEBOT.Domain.Common.Models;
using Marten;
using Newtonsoft.Json;

namespace EECEBOT.Infrastructure.Persistence.Interceptors;

public sealed class ConvertDomainEventsToOutboxMessagesInterceptor : DocumentSessionListenerBase
{
    public override Task BeforeSaveChangesAsync(IDocumentSession session, CancellationToken token)
    {
        var domainEvents = session.PendingChanges
            .AllChangedFor<AggregateRoot>()
            .SelectMany(aggregateRoot =>
            {
                var domainEvents = aggregateRoot.GetDomainEvents();
                aggregateRoot.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage
            {
                Id = Guid.NewGuid(),
                OccurredOnUtc = DateTime.UtcNow,
                Type = domainEvent.GetType().Name,
                Content = JsonConvert.SerializeObject(
                    domainEvent,
                    new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    })
            })
            .ToList();
        
        session.Store<OutboxMessage>(domainEvents);
        
        return base.BeforeSaveChangesAsync(session, token);
    }
}