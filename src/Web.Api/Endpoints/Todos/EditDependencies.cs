using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Create;
using Application.Todos.Dependency;
using Application.Todos.Edit;
using Application.Todos.EditSubItem;
using Domain.Todos;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class EditDependencies : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }

        public IEnumerable<RequestDependency> TaskDependencies { get; set; } = [];
    }

    public sealed class RequestDependency
    {
        public Guid Id { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id}/dependencies", async (
            Guid id,
            Request request,
            ICommandHandler<EditTodoDependenciesCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditTodoDependenciesCommand
            {
                TodoId = id,
                UserId = request.UserId,
                Dependencies = [.. request.TaskDependencies.Select(x => x.Id)]
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
