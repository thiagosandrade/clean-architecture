using Application.Todos.Attachments;
using Domain.API;
using Infrastructure.Extensions;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Todos;

internal sealed class DownloadAttachment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/{id:guid}/attachments/{attachmentId:guid}/download", async (
            Guid id,
            Guid attachmentId,
            Guid userId,
            IQueryHandler<GetAttachmentFileQuery, AttachmentFileResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttachmentFileQuery(id, attachmentId, userId);

            Result<AttachmentFileResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                attachment => Results.File(attachment.Data, attachment.ContentType, attachment.OriginalFileName),
                CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
