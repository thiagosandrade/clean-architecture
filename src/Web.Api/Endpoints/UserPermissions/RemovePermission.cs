using Application.Users.RemovePermission;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

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
