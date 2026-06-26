using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Application.OpenAI.Embeddings;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace Application.OpenAI.Enrichment;

public class CategoryEnrichmentService(OpenAIClient client, IConfiguration configuration) : ICategoryEnrichmentService
{
    public async Task<IReadOnlyCollection<string>> EnrichAsync(string description, IEnumerable<string> userLabels, CancellationToken cancellationToken = default)
    {
        string model = configuration["AIConfig:ChatModel"];

        ChatClient chatClient = client.GetChatClient(model);

        string prompt = PromptBuilder.TodoCategories(description);

        List<ChatMessage> messages =
        [
            new SystemChatMessage("You are a categorization engine. Return valid JSON only."),
            new UserChatMessage(prompt)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, options, cancellationToken);

        string content = result.Value.Content[0].Text;

        CategoryResponse response = JsonSerializer.Deserialize<CategoryResponse>(content)!;

        var categories = userLabels
                .Concat(response.Categories)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

        return categories;
    }

    private sealed class CategoryResponse
    {
        public List<string> Categories { get; set; } = [];
    }
}
