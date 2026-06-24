namespace Application.OpenAI.Parser;

public interface IParseTodoEnrichmentService
{
    Task<TodoExtractorResponse> EnrichAsync(string prompt, CancellationToken cancellationToken = default);
}
