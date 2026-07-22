using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Application.RabbitMq.Extensions;

public sealed record RabbitMqSubscription(string Exchange, string Queue, string RoutingKey, Type EventType);

public static class RabbitMqExtensions
{
    public static IServiceCollection AddRabbitMqConsumer<TEvent, THandler>(this IServiceCollection services, string queue)
        where TEvent : class, IIntegrationEvent
        where THandler : class, IIntegrationEventHandler<TEvent>
    {
        services.AddScoped<IIntegrationEventHandler<TEvent>, THandler>();

        RabbitMqEventAttribute attribute = typeof(TEvent)
            .GetCustomAttribute<RabbitMqEventAttribute>()
            ?? throw new InvalidOperationException(
                $"{typeof(TEvent).Name} is missing RabbitMqEventAttribute.");

        services.AddSingleton(new RabbitMqSubscription(
            attribute.Exchange,
            queue,
            attribute.RoutingKey,
            typeof(TEvent)));

        return services;
    }

    private static readonly ConcurrentDictionary<Type, RabbitMqEventAttribute> Cache = new();

    public static RabbitMqEventAttribute Get(Type type)
    {
        return Cache.GetOrAdd(type, static t =>
        {
            RabbitMqEventAttribute? attribute =
                t.GetCustomAttributes(typeof(RabbitMqEventAttribute), false)
                    .Cast<RabbitMqEventAttribute>()
                    .SingleOrDefault() ?? throw new InvalidOperationException(
                    $"The event '{t.Name}' is missing the [{nameof(RabbitMqEventAttribute)}] attribute.");

            return attribute;
        });
    }
}
