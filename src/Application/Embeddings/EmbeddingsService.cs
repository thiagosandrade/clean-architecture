using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Embeddings;

namespace Application.Embeddings;

public class EmbeddingsService(OpenAIClient client, IConfiguration configuration) : IEmbeddingsService
{
    public async Task<float[]> GenerateEmbeddingsAsync(string description)
    {
        string model = configuration["AIConfig:Model"];

        EmbeddingClient embeddingClient = client.GetEmbeddingClient(model);

        ClientResult<OpenAIEmbedding> response = await embeddingClient.GenerateEmbeddingAsync(description);

        float[] vector = response.Value.ToFloats().ToArray();

        return vector;
    }
}
