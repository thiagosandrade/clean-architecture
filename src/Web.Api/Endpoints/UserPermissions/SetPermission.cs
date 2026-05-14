using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Users.Register;
using Application.Users.SetPermission;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.UserPermissions;

internal sealed class SetPermission : IEndpoint
{
    public sealed record Request(Guid UserId, Guid PermissionId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("user-permission/set", async (
            Request request,
            ICommandHandler<SetPermissionToUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SetPermissionToUserCommand(request.PermissionId, request.UserId);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.UsersAccess)
        .WithTags(Tags.UsersPermission);
    }
}
