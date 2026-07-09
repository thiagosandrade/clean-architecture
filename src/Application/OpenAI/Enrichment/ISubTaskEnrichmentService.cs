using Application.Todos.Breakdown;

namespace Application.OpenAI.Enrichment;

public interface ISubTaskEnrichmentService
{
    Task<IReadOnlyCollection<string>> GenerateSubTasksAsync(string description, BreakdownStrategy strategy, BreakdownComplexity complexity, CancellationToken cancellationToken = default);
}
