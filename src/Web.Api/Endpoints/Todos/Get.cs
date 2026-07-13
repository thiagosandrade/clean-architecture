using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.GetByDescription;
using Domain.Todos;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

public sealed class GetTodosRequest
{
    public Guid UserId { get; init; }

    public int Page { get; init; } = 1;

    public int Size { get; init; } = 20;

    public string PropertyName { get; init; } = "CreatedAt";

    public bool Descending { get; init; }

    public Priority? Priority { get; init; }

    public DateOnly? DueDateFrom { get; init; }
    
    public DateOnly? DueDateTo { get; init; }

    public bool? IsCompleted { get; init; }

}

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos", async (
            [AsParameters] GetTodosRequest request,
            IQueryHandler <GetTodosQuery, PagedResponse<TodoResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTodosQuery(
                request.UserId,
                new Paginated(request.Page, request.Size),
                new Sorted(request.PropertyName, request.Descending),
                new TodoFilter
                {
                    Priority = request.Priority,
                    DueDateFrom = request.DueDateFrom,
                    DueDateTo = request.DueDateTo,
                    IsCompleted = request.IsCompleted
                }
            );

            Result<PagedResponse<TodoResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
