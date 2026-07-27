using Application.Todos.Get;
using Domain;
using Domain.API;
using Domain.Todos;
using Infrastructure.Extensions;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

public sealed class GetTodosRequest
{
    public Guid UserId { get; init; }

    public int Page { get; init; } = 1;

    public int Size { get; init; } = 20;

    public string PropertyName { get; init; } = "CreatedOn";

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
            IQueryHandler <GetTodosQuery, PagedResponse<TodoItemResponse>> handler,
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

            Result<PagedResponse<TodoItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
