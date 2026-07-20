using Application.Permissions.GetAll;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Permissions;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("permissions", async (
            IQueryHandler<GetAllQuery, List<PermissionResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllQuery();

            Result<List<PermissionResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.UsersAccess, PermissionsConstants.PermissionAccess)
        .WithTags(Tags.Permissions);
    }
}
