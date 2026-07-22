using System;
using System.Collections.Generic;
using System.Text;
using Application.Elastic.Services;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Elastic.Clients.Elasticsearch.Inference;

namespace Application.RabbitMq.Consumers;

internal sealed class TodoCreatedIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoCreatedIntegrationEvent>
{
    public async Task Handle(TodoCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoUpdatedIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoUpdatedIntegrationEvent>
{
    public async Task Handle(TodoUpdatedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoDeletedIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoDeletedIntegrationEvent>
{
    public async Task Handle(TodoDeletedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoAttachmentIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoAttachmentIntegrationEvent>
{
    public async Task Handle(TodoAttachmentIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoBreakdownIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoBreakdownIntegrationEvent>
{
    public async Task Handle(TodoBreakdownIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoDependencyIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoDependencyIntegrationEvent>
{
    public async Task Handle(TodoDependencyIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}

internal sealed class TodoRewriteIntegrationEventHandler(IElasticTodoSearchService elasticSearchService) : IIntegrationEventHandler<TodoRewriteIntegrationEvent>
{
    public async Task Handle(TodoRewriteIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        await elasticSearchService.IndexTodoAsync(integrationEvent.TodoId, cancellationToken);
    }
}
