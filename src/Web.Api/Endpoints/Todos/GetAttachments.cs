using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Attachments;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Todos;

internal sealed class GetAttachments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("todos/{id:guid}/attachments", async (
            Guid id,
            Guid userId,
            IQueryHandler<GetAttachmentsQuery, AttachmentsResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetAttachmentsQuery(id, userId);

            Result<AttachmentsResponse> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos);
    }
}
