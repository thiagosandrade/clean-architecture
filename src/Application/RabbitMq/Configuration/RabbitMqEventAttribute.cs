using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application.RabbitMq.Configuration;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class RabbitMqEventAttribute : Attribute
{
    public RabbitMqEventAttribute(
        string exchange,
        string routingKey)
    {
        Exchange = exchange;
        RoutingKey = routingKey;
    }

    public string Exchange { get; }

    public string RoutingKey { get; }
}
