namespace Application.OpenAI.Embeddings;

public interface IEmbeddingsService
{
    Task<float[]> GenerateEmbeddingsAsync(string description);
}
