using Application.Users.UpdateProfile;
using Domain.API;
using Infrastructure.Extensions;
using SharedKernel.Abstractions.Messaging;

namespace Web.Api.Endpoints.Users;

public sealed record UpdateUserRequest
{
    public string Email { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}

internal sealed class Put : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            ICommandHandler<UpdateUserProfileCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserProfileCommand(id, request.Email, request.FirstName, request.LastName);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users);
    }
}
