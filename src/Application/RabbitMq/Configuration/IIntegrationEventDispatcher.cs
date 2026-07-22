using Microsoft.Extensions.DependencyInjection;

namespace Application.RabbitMq.Configuration;

public interface IIntegrationEventDispatcher
{
    Task DispatchAsync(Type eventType, object integrationEvent, CancellationToken cancellationToken);
}

internal sealed class IntegrationEventDispatcher(IServiceProvider provider) : IIntegrationEventDispatcher
{
    public async Task DispatchAsync(Type eventType, object integrationEvent, CancellationToken cancellationToken)
    {
        Type handlerType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

        dynamic handler = provider.GetRequiredService(handlerType);

        await handler.Handle((dynamic)integrationEvent, cancellationToken);
    }
}
