namespace Application.OpenAI.Enrichment;

public sealed record EnrichmentResult(IReadOnlyCollection<string> Categories, float[] Embedding);
