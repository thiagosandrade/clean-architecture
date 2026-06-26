namespace Application.OpenAI.Enrichment;

public interface ICategoryEnrichmentService
{
    Task<IReadOnlyCollection<string>> EnrichAsync(string description, IEnumerable<string> userLabels, CancellationToken cancellationToken = default);
}
