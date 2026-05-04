using Application.Abstractions.Messaging;
using Application.Users.Remove;
using Application.Users.SetPermission;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Users;

internal sealed class Remove : IEndpoint
{
    public sealed record Request(Guid UserId);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/remove", async (
            Request request,
            ICommandHandler<RemoveUserCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new RemoveUserCommand(
                request.UserId);
            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
