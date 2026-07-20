using Application.Todos.GetBy;
using Domain;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

public sealed class GetSearchByRequest
{
    public Guid UserId { get; set; }
    public string Description { get; set; }
}

internal sealed class GetBy : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos/searchby", async (
            [FromBody] GetSearchByRequest request,
            IQueryHandler<GetTodoItemQuery, List<GetTodoItemQueryResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new GetTodoItemQuery(request.UserId, request.Description);

            Result<List<GetTodoItemQueryResponse>> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Todos)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
