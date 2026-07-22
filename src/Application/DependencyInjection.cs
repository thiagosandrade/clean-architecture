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

        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddScoped<IEmbeddingsService, EmbeddingsService>();
        services.AddScoped<ICategoryEnrichmentService, CategoryEnrichmentService>();
        services.AddScoped<IParseTodoEnrichmentService, ParseTodoEnrichmentService>();
        services.AddScoped<ISubTaskEnrichmentService, SubTaskEnrichmentService>();
        services.AddScoped<IRewriteEnrichmentService, RewriteEnrichmentService>();
        services.AddScoped<ITodoActivityService, TodoActivityService>();

        services.AddOpenAI(configuration);

        services.AddElasticsearch(configuration);

        services.AddRabbitMq(configuration);

        return services;
    }

    public static IServiceCollection AddDataProcessorApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddElasticsearch(configuration);

        services.AddRabbitMq(configuration);

        services.AddRabbitMqConsumer<TodoCreatedIntegrationEvent, TodoCreatedIntegrationEventHandler>(queue: "todo-created");

        services.AddRabbitMqConsumer<TodoUpdatedIntegrationEvent, TodoUpdatedIntegrationEventHandler>(queue: "todo-updated");

        services.AddRabbitMqConsumer<TodoDeletedIntegrationEvent, TodoDeletedIntegrationEventHandler>(queue: "todo-deleted");

        services.AddRabbitMqConsumer<TodoAttachmentIntegrationEvent, TodoAttachmentIntegrationEventHandler>(queue: "todo-attachment");

        services.AddRabbitMqConsumer<TodoBreakdownIntegrationEvent, TodoBreakdownIntegrationEventHandler>(queue: "todo-breakdown");

        services.AddRabbitMqConsumer<TodoDependencyIntegrationEvent, TodoDependencyIntegrationEventHandler>(queue: "todo-dependency");

        services.AddRabbitMqConsumer<TodoRewriteIntegrationEvent, TodoRewriteIntegrationEventHandler>(queue: "todo-rewrite");

        services.AddRabbitMqConsumer<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>(queue: "user-created");

        services.AddRabbitMqConsumer<UserUpdatedIntegrationEvent, UserUpdatedIntegrationEventHandler>(queue: "user-updated");

        services.AddRabbitMqConsumer<UserDeletedIntegrationEvent, UserDeletedIntegrationEventHandler>(queue: "user-deleted");

        services.AddHostedService<RebuildIndexHostedService>();

        return services;
    }
}
