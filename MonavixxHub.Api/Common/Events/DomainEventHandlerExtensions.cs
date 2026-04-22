using System.Reflection;

namespace MonavixxHub.Api.Common.Events;

public static class DomainEventHandlerExtensions
{
    public static IServiceCollection AddDomainEventHandlers(this IServiceCollection services, Assembly assembly)
    {
        var handlers = assembly.GetTypes();
        foreach (var handler in handlers)
        {
            var interfaces = handler.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
            foreach (var @interface in interfaces)
            {
                services.AddScoped(@interface, handler);
            }
        }
        return services;
    }
}