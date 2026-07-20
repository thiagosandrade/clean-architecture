using Application.Todos.Breakdown;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

internal sealed class Breakdown : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public BreakdownComplexity Complexity { get; set; }
        public BreakdownStrategy Strategy { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/ai/{id:guid}/breakdown", async (
            Guid id,
            Request request,
            ICommandHandler<TaskBreakdownCommand, BreakdownResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new TaskBreakdownCommand()
            {
                UserId = request.UserId,
                TodoId = id,
                Complexity = request.Complexity,
                Strategy = request.Strategy
            };

            Result<BreakdownResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
