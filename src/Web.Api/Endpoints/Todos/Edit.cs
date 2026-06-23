using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Create;
using Application.Todos.Edit;
using Domain.Todos;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Edit : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = [];
        public int Priority { get; set; }
        public bool IsCompleted { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("todos/{id}", async (
            Guid id,
            Request request,
            ICommandHandler<EditTodoCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditTodoCommand
            {
                Id = id,
                UserId = request.UserId,
                Description = request.Description,
                DueDate = request.DueDate,
                Labels = request.Labels,
                Priority = (Priority)request.Priority,
                IsCompleted = request.IsCompleted
            };

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
