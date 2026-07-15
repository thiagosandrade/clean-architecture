using Application.Abstractions.Constants;
using Application.Abstractions.Messaging;
using Application.Todos.Attachments;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Web.Api.Endpoints.Todos;

internal sealed class CreateAttachment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("todos/{id:guid}/attachments", async (
            Guid id,
            IFormFile file,
            Guid userId,
            ICommandHandler<CreateAttachmentCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            if (file is null)
            {
                return Results.BadRequest();
            }

            string storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, cancellationToken);
            byte[] data = ms.ToArray();

            var command = new CreateAttachmentCommand(
                id,
                userId,
                file.FileName,
                storedFileName,
                file.ContentType ?? string.Empty,
                file.Length,
                data);

            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .HasPermission(PermissionsConstants.TodoAccess)
        .WithTags(Tags.Todos)
        .DisableAntiforgery();
    }
}
