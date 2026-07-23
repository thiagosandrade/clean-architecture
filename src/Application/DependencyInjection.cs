using Application.Elastic;
using Application.Elastic.Extensions;
using Application.Elastic.Services;
using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Application.OpenAI.Extensions;
using Application.OpenAI.Parser;
using Application.RabbitMq.Consumers;
using Application.RabbitMq.Events;
using Application.RabbitMq.Extensions;
using Application.Todos.Activities.Log;
using Domain.DomainEvents;
using Elastic.Clients.Elasticsearch;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using SharedKernel.Abstractions.Behaviors;
using SharedKernel.Abstractions.Messaging;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddBackendApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddCQRSAssemblies(services);

        AddDomainHandlers(services);

        services.AddOpenAI(configuration);

        services.AddScoped<IEmbeddingsService, EmbeddingsService>();
        services.AddScoped<ICategoryEnrichmentService, CategoryEnrichmentService>();
        services.AddScoped<IParseTodoEnrichmentService, ParseTodoEnrichmentService>();
        services.AddScoped<ISubTaskEnrichmentService, SubTaskEnrichmentService>();
        services.AddScoped<IRewriteEnrichmentService, RewriteEnrichmentService>();
        services.AddScoped<ITodoActivityService, TodoActivityService>();

        services.AddElasticsearch(configuration);

        services.AddRabbitMq(configuration);

        return services;
    }

    public static IServiceCollection AddDataProcessorApplication(this IServiceCollection services, IConfiguration configuration)
    {
        AddCQRSAssemblies(services);

        AddDomainHandlers(services);

        services.AddOpenAI(configuration);

        services.AddScoped<IEmbeddingsService, EmbeddingsService>();
        services.AddScoped<ICategoryEnrichmentService, CategoryEnrichmentService>();
        services.AddScoped<ITodoActivityService, TodoActivityService>();

        services.AddElasticsearch(configuration);

        services.AddRabbitMq(configuration);

        services.AddRabbitMqConsumer<TodoEmbeddingRequestedIntegrationEvent, TodoEmbeddingRequestedIntegrationEventHandler>(queue: "dataprocessor--todo-embedding-requested");

        services.AddRabbitMqConsumer<TodoCreatedIntegrationEvent, TodoCreatedIntegrationEventHandler>(queue: "dataprocessor--todo-created");

        services.AddRabbitMqConsumer<TodoUpdatedIntegrationEvent, TodoUpdatedIntegrationEventHandler>(queue: "dataprocessor--todo-updated");

        services.AddRabbitMqConsumer<TodoDeletedIntegrationEvent, TodoDeletedIntegrationEventHandler>(queue: "dataprocessor--todo-deleted");

        services.AddRabbitMqConsumer<TodoAttachmentIntegrationEvent, TodoAttachmentIntegrationEventHandler>(queue: "dataprocessor--todo-attachment-added");

        services.AddRabbitMqConsumer<TodoBreakdownIntegrationEvent, TodoBreakdownIntegrationEventHandler>(queue: "dataprocessor--todo-breakdown-executed");

        services.AddRabbitMqConsumer<TodoDependencyIntegrationEvent, TodoDependencyIntegrationEventHandler>(queue: "dataprocessor--todo-dependency-updated");

        services.AddRabbitMqConsumer<TodoRewriteIntegrationEvent, TodoRewriteIntegrationEventHandler>(queue: "dataprocessor--todo-rewrite-executed");

        services.AddRabbitMqConsumer<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>(queue: "dataprocessor--user-created");

        services.AddRabbitMqConsumer<UserUpdatedIntegrationEvent, UserUpdatedIntegrationEventHandler>(queue: "dataprocessor--user-updated");

        services.AddRabbitMqConsumer<UserDeletedIntegrationEvent, UserDeletedIntegrationEventHandler>(queue: "dataprocessor--user-deleted");

        services.AddHostedService<RebuildIndexHostedService>();

        return services;
    }

    private static void AddDomainHandlers(IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }

    private static void AddCQRSAssemblies(IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());

        services.Decorate(typeof(ICommandHandler<,>), typeof(ValidationDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(ValidationDecorator.CommandBaseHandler<>));

        services.Decorate(typeof(IQueryHandler<,>), typeof(LoggingDecorator.QueryHandler<,>));
        services.Decorate(typeof(ICommandHandler<,>), typeof(LoggingDecorator.CommandHandler<,>));
        services.Decorate(typeof(ICommandHandler<>), typeof(LoggingDecorator.CommandBaseHandler<>));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
    }
}
