using Application.Elastic.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Elastic;

internal sealed class RebuildIndexHostedService(IServiceScopeFactory scopeFactory, ILogger<RebuildIndexHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Elasticsearch bootstrap...");

        using IServiceScope scope = scopeFactory.CreateScope();

        IElasticTodoSearchService todoSearchService = scope.ServiceProvider.GetRequiredService<IElasticTodoSearchService>();

        IElasticUserSearchService userSearchService = scope.ServiceProvider.GetRequiredService<IElasticUserSearchService>();

        await todoSearchService.RebuildTodoIndexAsync(cancellationToken);

        await userSearchService.RebuildUserIndexAsync(cancellationToken);

        logger.LogInformation("Elasticsearch bootstrap completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
