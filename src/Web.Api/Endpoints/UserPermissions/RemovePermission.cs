using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Users.RemovePermission;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.UserPermissions;

internal sealed class RemovePermission : IEndpoint
{
    public sealed record Request(Guid UserId, Guid PermissionId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user-permission/remove", async (
            Request request,
            ICommandHandler<RemovePermissionFromUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemovePermissionFromUserCommand(request.PermissionId, request.UserId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.UsersAccess)
        .WithTags(Tags.UsersPermission);
    }
}
