using Application.Elastic.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Application.Elastic;

internal sealed class RebuildIndexHostedService(
    IElasticTodoSearchService elasticTodoSearchService,
    IElasticUserSearchService elasticUsersSearchService,
    ILogger<RebuildIndexHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Elasticsearch bootstrap...");

        await elasticTodoSearchService.RebuildTodoIndexAsync(cancellationToken);
        await elasticUsersSearchService.RebuildUsersAsync(cancellationToken);

        logger.LogInformation("Elasticsearch bootstrap completed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
