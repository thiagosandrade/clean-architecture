using Application.Todos.EditSubItem;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

internal sealed class EditSubItems : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }

        public Guid TodoId { get; set; }

        public IEnumerable<RequestSubItem> TodoSubItems { get; set; } = [];
    }

    public sealed class RequestSubItem
    {
        public Guid Id { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public int Order { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id}/subitems", async (
            Guid id,
            Request request,
            ICommandHandler<EditTodoSubItemsCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditTodoSubItemsCommand
            {
                TodoId = id,
                UserId = request.UserId,
                TodoSubItems = [.. request.TodoSubItems.Select(x => new TodoSubItemCommand
                {
                    Id = x.Id,
                    Description = x.Description,
                    IsCompleted = x.IsCompleted,
                    Order = x.Order
                })]
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
