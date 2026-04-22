using System.Collections.Concurrent;
using System.Reflection;

namespace MonavixxHub.Api.Common.Events;

public class DomainEventPublisher(IServiceProvider serviceProvider) : IDomainEventPublisher
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> DispatchCache = new();

    private static readonly MethodInfo DispatchMethod = typeof(DomainEventPublisher)
        .GetMethod(nameof(DispatchAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

    /// <inheritdoc />
    public Task PublishAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        var eventType = @event.GetType();
        if (!DispatchCache.TryGetValue(eventType, out var dispatchMethod))
            DispatchCache[eventType] = dispatchMethod = DispatchMethod.MakeGenericMethod(eventType);
        return (Task)dispatchMethod.Invoke(this, [@event, cancellationToken])!;
    }

    private async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent
    {
        foreach (var handler in serviceProvider.GetServices<IDomainEventHandler<TEvent>>())
        {
            await handler.HandleAsync(@event, cancellationToken);
        }
    }
}