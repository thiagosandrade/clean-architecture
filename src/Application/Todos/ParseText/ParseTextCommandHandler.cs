using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.OpenAI;
using Application.OpenAI.Parser;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.ParseText;

internal sealed class ParseTextCommandHandler(IParseTodoEnrichmentService parseTodoEnrichmentService): ICommandHandler<ParseTextCommand, TodoExtractorResponse>
{
    public async Task<Result<TodoExtractorResponse>> Handle(ParseTextCommand command, CancellationToken cancellationToken)
    {
        string promptToParse = PromptBuilder.ParseTextForTodo(command.Description, command.UserId);

        TodoExtractorResponse response = await parseTodoEnrichmentService.EnrichAsync(promptToParse, cancellationToken);

        return response;
    }
}
