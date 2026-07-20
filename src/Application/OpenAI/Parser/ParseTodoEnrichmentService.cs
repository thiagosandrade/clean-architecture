using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace Application.OpenAI.Parser;

public class ParseTodoEnrichmentService(OpenAIClient client, IConfiguration configuration) : IParseTodoEnrichmentService
{
    public async Task<TodoExtractorResponse> EnrichAsync(string prompt, CancellationToken cancellationToken = default)
    {
        string model = configuration["AIConfig:ChatModel"];

        ChatClient chatClient = client.GetChatClient(model);

        List<ChatMessage> messages =
        [
            new SystemChatMessage("You are a todo extractor from a text engine. Return valid JSON only."),
            new UserChatMessage(prompt)
        ];

        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        ClientResult<ChatCompletion> result = await chatClient.CompleteChatAsync(messages, options, cancellationToken);

        string content = result.Value.Content[0].Text;

        TodoExtractorResponse response = JsonSerializer.Deserialize<TodoExtractorResponse>(content)!;

        return response;
    }
}
