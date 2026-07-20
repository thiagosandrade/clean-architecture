using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Dashboard;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Dashboard;

public sealed class GetDashboardRequest
{
    public Guid UserId { get; init; }
}

internal sealed class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dashboard", async (
            [FromBody] GetDashboardRequest request,
            IQueryHandler<GetDashboardQuery, DashboardResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetDashboardQuery(request.UserId);

            Result<DashboardResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Dashboard)
        .HasPermission(PermissionsConstants.TodoAccess);
    }
}
