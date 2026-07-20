using Application.Abstractions.Authentication;
using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Breakdown;
using Application.Todos.Complete;
using Application.Todos.Rewrite;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Rewrite : IEndpoint
{
    public sealed record Request
    {
        public Guid UserId { get; init; }

        public string Description { get; init; } = string.Empty;

        public RewriteStyle Style { get; init; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos/ai/{id:guid}/rewrite", async (
            Guid id,
            [FromBody] Request request,
            ICommandHandler<SubtaskRewriteCommand, SubtaskRewriteResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SubtaskRewriteCommand()
            {
                TodoId = id,
                UserId = request.UserId,
                Description = request.Description,
                Style = request.Style
            };

            Result<SubtaskRewriteResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
