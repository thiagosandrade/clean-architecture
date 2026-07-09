using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Application.OpenAI.Embeddings;
using Application.Todos.Rewrite;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace Application.OpenAI.Enrichment;

public class RewriteEnrichmentService(OpenAIClient client, IConfiguration configuration) : IRewriteEnrichmentService
{
    public async Task<SubtaskRewriteResponse> RewriteAsync(string description, RewriteStyle style, CancellationToken cancellationToken)
    {
        string model = configuration["AIConfig:ChatModel"];

        ChatClient chatClient = client.GetChatClient(model);

        string prompt = PromptBuilder.SubtaskRewrite(description, style.ToString());

        List<ChatMessage> messages =
        [
            new SystemChatMessage("You description rewrite engine. Return valid json only."),
            new UserChatMessage(prompt)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, options, cancellationToken);

        string content = result.Value.Content[0].Text;

        RewriteResponse response = JsonSerializer.Deserialize<RewriteResponse>(content)!;

        return new SubtaskRewriteResponse(response.Description);
    }

    private sealed class RewriteResponse
    {
        public string Description { get; set; } = string.Empty;
    }
}
