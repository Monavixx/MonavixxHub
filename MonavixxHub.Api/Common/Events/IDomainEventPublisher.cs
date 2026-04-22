namespace MonavixxHub.Api.Common.Events;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default);
}