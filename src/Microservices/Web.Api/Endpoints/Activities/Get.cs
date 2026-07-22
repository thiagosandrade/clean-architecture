using Application.Todos.Activities.Get;
using Domain.API;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using Web.Api.Extensions;

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
