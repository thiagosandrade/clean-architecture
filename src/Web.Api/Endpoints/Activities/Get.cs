using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Activities.Get;
using Domain.Todos;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Endpoints.Todos;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Activities;

public sealed class GetTodoActivitiesRequest
{
    public Guid UserId { get; init; }
}

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("activities/todo/{id:Guid}", async (
            Guid id,
            [FromBody] GetTodoActivitiesRequest request,
            IQueryHandler<GetTodoActivitiesQuery, GetTodoActivityResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTodoActivitiesQuery(
                id,
                request.UserId
            );

            Result<GetTodoActivityResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Activities)
        .HasPermission(PermissionsConstants.ActivityAccess);
    }
}
