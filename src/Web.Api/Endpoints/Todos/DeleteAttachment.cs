using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Attachments;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class DeleteAttachment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("todos/{id:guid}/attachments/{attachmentId:guid}", async (
            Guid id,
            Guid attachmentId,
            Guid userId,
            ICommandHandler<DeleteAttachmentCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteAttachmentCommand(id, attachmentId, userId);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
