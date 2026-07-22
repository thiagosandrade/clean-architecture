using System;
using System.Collections.Generic;
using System.Text;
using Application.Elastic.Services;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Elastic.Clients.Elasticsearch.Inference;

namespace Application.RabbitMq.Consumers;

internal sealed class UserCreatedIntegrationEventHandler(IElasticUserSearchService elasticSearchService) : IIntegrationEventHandler<UserCreatedIntegrationEvent>
{
    public async Task Handle(UserCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexUserAsync(integrationEvent.UserId, cancellationToken);
    }
}

internal sealed class UserUpdatedIntegrationEventHandler(IElasticUserSearchService elasticSearchService) : IIntegrationEventHandler<UserUpdatedIntegrationEvent>
{
    public async Task Handle(UserUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexUserAsync(integrationEvent.UserId, cancellationToken);
    }
}

internal sealed class UserDeletedIntegrationEventHandler(IElasticUserSearchService elasticSearchService) : IIntegrationEventHandler<UserDeletedIntegrationEvent>
{
    public async Task Handle(UserDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexUserAsync(integrationEvent.UserId, cancellationToken);
    }
}
