using Application.Abstractions.Authentication;
using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Breakdown;
using Application.Todos.Complete;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Breakdown : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id:guid}/breakdown", async (
            Guid id,
            Request request,
            ICommandHandler<TaskBreakdownCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new TaskBreakdownCommand()
            {
                UserId = request.UserId,
                TodoId = id
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
