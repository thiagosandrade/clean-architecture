using Application.Users.GetAll;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Users;

internal sealed class GetAll : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("users", async (
            IQueryHandler<GetAllQuery, List<UserResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAllQuery();

            Result<List<UserResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.UsersAccess)
        .WithTags(Tags.Users);
    }
}
