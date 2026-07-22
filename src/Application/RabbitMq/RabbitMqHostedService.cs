using System.Text.Json;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Application.RabbitMq;

internal sealed class RabbitMqHostedService(
    RabbitMqConnection connection, 
    IIntegrationEventDispatcher eventDispatcher, 
    IEnumerable<RabbitMqSubscription> subscriptions, 
    ILogger<RabbitMqHostedService> logger) 
    : BackgroundService
{
    private readonly List<IChannel> _channels = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RabbitMQ Hosted Service starting");

        IConnection rabbitConnection = await connection.GetAsync(stoppingToken);

        logger.LogInformation("Found {Count} RabbitMQ subscriptions", subscriptions.Count());

        foreach (RabbitMqSubscription subscription in subscriptions)
        {
            logger.LogInformation($"Creating queue {subscription.Queue} bound to {subscription.Exchange} ({subscription.RoutingKey})");

            IChannel channel = await rabbitConnection.CreateChannelAsync(
                cancellationToken: stoppingToken);

            _channels.Add(channel);

            await channel.ExchangeDeclareAsync(
                exchange: subscription.Exchange,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await channel.QueueDeclareAsync(
                queue: subscription.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: stoppingToken);

            await channel.QueueBindAsync(
                queue: subscription.Queue,
                exchange: subscription.Exchange,
                routingKey: subscription.RoutingKey,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, args) =>
            {
                try
                {
                    object? integrationEvent = JsonSerializer.Deserialize(
                        args.Body.Span,
                        subscription.EventType);

                    if (integrationEvent is null)
                    {
                        await channel.BasicNackAsync(
                            args.DeliveryTag,
                            multiple: false,
                            requeue: false,
                            cancellationToken: stoppingToken);

                        return;
                    }

                    await eventDispatcher.DispatchAsync(
                        subscription.EventType,
                        integrationEvent,
                        stoppingToken);

                    await channel.BasicAckAsync(
                        args.DeliveryTag,
                        multiple: false,
                        cancellationToken: stoppingToken);
                }
                catch (Exception)
                {
                    await channel.BasicNackAsync(
                        args.DeliveryTag,
                        multiple: false,
                        requeue: true,
                        cancellationToken: stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: subscription.Queue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);
        }

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (IChannel channel in _channels)
        {
            await channel.DisposeAsync();
        }

        _channels.Clear();

        await base.StopAsync(cancellationToken);
    }
}
