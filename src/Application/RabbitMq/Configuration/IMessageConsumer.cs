using System;
using System.Collections.Generic;
using System.Text;
using Application.Elastic;
using Application.RabbitMq.Events;
using Elastic.Clients.Elasticsearch.Inference;

namespace Application.RabbitMq.Configuration;

public interface IMessageConsumer<in T>
{
    Task ConsumeAsync(T message, CancellationToken cancellationToken);
}
