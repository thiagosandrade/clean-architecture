using System.Collections;
using System.Text.Json;
using Application.RabbitMq.Events;
using Application.RabbitMq.Extensions;
using RabbitMQ.Client;

namespace Application.RabbitMq.Configuration;

public interface IRabbitMqPublisher
{
    Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}

internal sealed class RabbitMqPublisher(RabbitMqConnection connection) : IRabbitMqPublisher
{
    public async Task PublishAsync<T>(T integrationEvent, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent
    {
        RabbitMqEventAttribute metadata = RabbitMqExtensions.Get(typeof(T));

        IConnection rabbitConnection = await connection.GetAsync(cancellationToken);

        await using IChannel channel = await rabbitConnection.CreateChannelAsync(
                cancellationToken: cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange: metadata.Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        byte[] body = JsonSerializer.SerializeToUtf8Bytes(integrationEvent);

        await channel.BasicPublishAsync(
            exchange: metadata.Exchange,
            routingKey: metadata.RoutingKey,
            mandatory: false,
            body: body,
            cancellationToken: cancellationToken);
    }
}
