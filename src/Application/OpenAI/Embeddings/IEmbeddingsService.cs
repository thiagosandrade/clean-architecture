namespace Application.OpenAI.Embeddings;

public interface IEmbeddingsService
{
    Task<float[]> GenerateEmbeddingsForSearchAsync(string description);
    Task<float[]> GenerateEmbeddingsAsync(string description, IReadOnlyCollection<string> categories);
}
