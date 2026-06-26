using System.ClientModel;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace Application.OpenAI.Enrichment;

internal sealed class SubTaskEnrichmentService(OpenAIClient client, IConfiguration configuration) : ISubTaskEnrichmentService
{
    public async Task<IReadOnlyCollection<string>> GenerateSubTasksAsync(
        string description,
        CancellationToken cancellationToken = default)
    {
        string model = configuration["AIConfig:ChatModel"];

        ChatClient chatClient = client.GetChatClient(model);

        List<ChatMessage> messages =
        [
            new SystemChatMessage(
                """
                You are a productivity assistant.

                Break TODOs into actionable subtasks.

                Rules:
                - Return between 3 and 8 subtasks.
                - Keep each subtask under 80 characters.
                - Preserve execution order.
                - Do not repeat the original task.
                - Return JSON only.
                """
            ),
            new UserChatMessage(PromptBuilder.SubTaskBreakdown(description))
        ];

        ChatCompletionOptions chatOptions = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        };

        ClientResult<ChatCompletion> result =
            await chatClient.CompleteChatAsync(
                messages,
                chatOptions,
                cancellationToken);

        string json = result.Value.Content[0].Text;

        SubTaskResponse? response =
            JsonSerializer.Deserialize<SubTaskResponse>(
                json,
                JsonSerializerOptions.Web);

        return response?.SubTasks ?? [];
    }

    private sealed class SubTaskResponse
    {
        public List<string> SubTasks { get; init; } = [];
    }
}
