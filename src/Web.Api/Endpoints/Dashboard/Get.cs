using Application.Dashboard;
using Domain;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

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
