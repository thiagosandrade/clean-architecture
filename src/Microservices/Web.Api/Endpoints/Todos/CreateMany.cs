using Application.Todos.Create;
using Domain.API;
using Domain.Todos;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Common;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

internal sealed class CreateMany : IEndpoint
{
    public sealed class Request
    {
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public List<string> Labels { get; set; } = [];
        public int Priority { get; set; }
    }

    public sealed class ListOfRequest 
    {
        public IEnumerable<Request> Items { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos-many", async (
            ListOfRequest request,
            ICommandHandler<CreateTodoCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var results = new List<Result<Guid>>();

            foreach (Request item in request.Items)
            {
                var command = new CreateTodoCommand
                {
                    UserId = item.UserId,
                    Description = item.Description,
                    DueDate = item.DueDate,
                    Labels = item.Labels,
                    Priority = (Priority)item.Priority
                };

                results.Add(await handler.Handle(command, cancellationToken));
            }

            var failures = results.Where(r => r.IsFailure).ToList();

            if (failures.Any())
            {
                var combinedError = new Error(
                    "BatchFailure",
                    string.Join(" | ", failures.Select(f => f.Error.Description)),
                    ErrorType.Failure
                );

                return CustomResults.Problem(Result.Failure(combinedError));
            }

            return Results.Ok(results.Select(r => r.Value));
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
