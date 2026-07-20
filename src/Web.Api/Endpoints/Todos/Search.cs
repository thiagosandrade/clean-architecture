using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Search;
using Domain.Users;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class Search : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/search", async (
            string searchtext,
            Guid userId,
            int page,
            int size,
            string? propertyName,
            bool? descending,
            IQueryHandler<SearchTodoItemsQuery, PagedResponse<SearchTodoItemResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            Sorted? sorting = null;

            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                sorting = new Sorted(propertyName,descending ?? false);
            }

            var query = new SearchTodoItemsQuery(
                searchtext,
                userId,
                new Paginated(page, size),
                sorting
            );

            Result<PagedResponse<SearchTodoItemResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
