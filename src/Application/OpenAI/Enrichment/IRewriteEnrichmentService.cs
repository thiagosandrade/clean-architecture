using Application.Todos.Rewrite;

namespace Application.OpenAI.Enrichment;

public interface IRewriteEnrichmentService
{
    Task<SubtaskRewriteResponse> RewriteAsync(string description, RewriteStyle style, CancellationToken cancellationToken);
}
