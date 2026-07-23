using System;
using System.Collections.Generic;
using System.Text;
using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Elastic.Services;
using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.Activities;
using Domain.DomainEvents;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;


namespace Application.RabbitMq.Consumers;

internal sealed class TodoEmbeddingRequestedIntegrationEventHandler(
    IApplicationDbContext context,
    ICategoryEnrichmentService categoryEnrichmentService,
    IRabbitMqPublisher publisher,
    IEmbeddingsService embeddingsService,
    IDateTimeProvider dateTimeProvider) : IIntegrationEventHandler<TodoEmbeddingRequestedIntegrationEvent>
{
    public async Task Handle(TodoEmbeddingRequestedIntegrationEvent integrationEvent, CancellationToken cancellationToken)
    {
        TodoItem todoItem = await context.TodoItems.FirstAsync(x => x.Id == integrationEvent.TodoId, cancellationToken: cancellationToken);

        IReadOnlyCollection<string> categories = await categoryEnrichmentService.EnrichAsync(todoItem.Description, todoItem.Labels, cancellationToken);

        float[] embedding = await embeddingsService.GenerateEmbeddingsAsync(todoItem.Description, [.. todoItem.Labels], categories);

        todoItem.Embedding = embedding.ToVector();
        todoItem.Categories = [.. categories];

        todoItem.UpdatedOn = dateTimeProvider.UtcNow;

        context.TodoItems.Update(todoItem);

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.EmbeddingsGenerated, "Embeddings Generated", todoItem.UserId));

        await context.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(new TodoCreatedIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);
    }
}

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
