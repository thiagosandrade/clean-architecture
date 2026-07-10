using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.TaskActivities.Get;
using Application.Todos.Get;
using Domain.Todos;
using SharedKernel;
using Web.Api.Endpoints.Todos;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Activities;

public sealed class GetTaskActivitiesRequest
{
    public Guid UserId { get; init; }
}

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("activities/task/{id:Guid}", async (
            Guid id,
            [AsParameters] GetTaskActivitiesRequest request,
            IQueryHandler<GetTaskActivitiesQuery, GetTaskActivityResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetTaskActivitiesQuery(
                id,
                request.UserId
            );

            Result<GetTaskActivityResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Activities)
        .HasPermission(PermissionsConstants.ActivityAccess);
    }
}
