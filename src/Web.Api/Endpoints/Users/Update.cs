using Application.Users.Update;
using Domain;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using SharedKernel.Extensions;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Users;

internal sealed class Update : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName, string Password);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/{id:guid}/update", async (
            Guid id,
            Request request,
            ICommandHandler<UpdateUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserCommand(
                id,
                request.Email,
                request.FirstName,
                request.LastName,
                request.Password);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.UsersAccess)
        .WithTags(Tags.Users);
    }
}
