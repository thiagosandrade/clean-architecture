namespace Application.OpenAI.Enrichment;

public interface IEnrichmentService
{
    Task<EnrichmentResult> EnrichAsync(string description, IEnumerable<string> userLabels, CancellationToken cancellationToken = default);
}
