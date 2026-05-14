using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Get;
using Application.Todos.Search;
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
            IQueryHandler<SearchTodosQuery, List<SearchTodoResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new SearchTodosQuery(searchtext);

            Result<List<SearchTodoResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
