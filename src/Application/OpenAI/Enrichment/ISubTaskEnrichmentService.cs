namespace Application.OpenAI.Enrichment;

public interface ISubTaskEnrichmentService
{
    Task<IReadOnlyCollection<string>> GenerateSubTasksAsync(string description, CancellationToken cancellationToken = default);
}
