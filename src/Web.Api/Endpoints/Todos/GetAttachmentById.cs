using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Attachments;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class GetAttachmentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/{id:guid}/attachments/{attachmentId:guid}", async (
            Guid id,
            Guid attachmentId,
            Guid userId,
            IQueryHandler<GetAttachmentByIdQuery, AttachmentResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttachmentByIdQuery(id, attachmentId, userId);

            Result<AttachmentResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
