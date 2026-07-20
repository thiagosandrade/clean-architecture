using Application.OpenAI.Embeddings;
using Application.OpenAI.Enrichment;
using Application.OpenAI.Parser;
using Application.Todos.Activities.Log;
using Domain;
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

        // OpenAI
        services.AddSingleton(new OpenAIClient(
            configuration["AIConfig:OpenAIKey"]
        ));

        return services;
    }

    public static IServiceCollection AddDataProcessorApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Scan(scan => scan.FromAssembliesOf(typeof(DependencyInjection))
            .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);


        return services;
    }
}
