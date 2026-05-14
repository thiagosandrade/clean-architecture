namespace Application.Embeddings;

public interface IEmbeddingsService
{
    Task<float[]> GenerateEmbeddingsAsync(string description);
}
