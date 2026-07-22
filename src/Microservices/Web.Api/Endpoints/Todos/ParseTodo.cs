using Application.OpenAI.Parser;
using Application.Todos.ParseText;
using Domain.API;
using Infrastructure.Extensions;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

internal sealed class ParseTodo : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos/parse", async (
            Request request,
            ICommandHandler<ParseTextCommand, TodoExtractorResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ParseTextCommand
            {
                UserId = request.UserId,
                Description = request.Description
            };

            Result<TodoExtractorResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
