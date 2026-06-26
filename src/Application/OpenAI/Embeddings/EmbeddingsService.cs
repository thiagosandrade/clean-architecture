using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;

namespace Application.OpenAI.Embeddings;

public class EmbeddingsService(OpenAIClient client, IConfiguration configuration) : IEmbeddingsService
{
    public Task<float[]> GenerateEmbeddingsForSearchAsync(string description)
    {
        return GenerateEmbeddingsAsync(description, []);
    }

    public async Task<float[]> GenerateEmbeddingsAsync(string description, IReadOnlyCollection<string> categories)
    {
        string model = configuration["AIConfig:EmbeddingsModel"];

        EmbeddingClient embeddingClient = client.GetEmbeddingClient(model);

        string embeddingText = PromptBuilder.TodoDescription(description, categories);

        ClientResult<OpenAIEmbedding> response = await embeddingClient.GenerateEmbeddingAsync(embeddingText);

        float[] vector = response.Value.ToFloats().ToArray();

        return vector;
    }
}
