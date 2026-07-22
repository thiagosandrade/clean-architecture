using System;
using System.Collections.Generic;
using System.Text;
using Application.RabbitMq.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.RabbitMq.Extensions;

public static class AddRabbitMqExtensions
{
    public static void AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMQ"));

        services.AddSingleton<RabbitMqConnection>();

        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();

        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();

        services.AddHostedService<RabbitMqHostedService>();
    }
}
