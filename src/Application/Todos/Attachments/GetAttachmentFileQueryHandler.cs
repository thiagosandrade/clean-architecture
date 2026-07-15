using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Domain.Users;
using Domain.Todos;

namespace Application.Todos.Attachments;

internal sealed class GetAttachmentFileQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetAttachmentFileQuery, AttachmentFileResponse>
{
    public async Task<Result<AttachmentFileResponse>> Handle(GetAttachmentFileQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<AttachmentFileResponse>(UserErrors.Unauthorized());
        }

        TaskAttachment? attachment = await context.TaskAttachments
            .SingleOrDefaultAsync(a => a.Id == query.AttachmentId && a.TodoItemId == query.TodoId, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure<AttachmentFileResponse>(TodoItemErrors.AttachmentNotFound(query.AttachmentId));
        }

        var response = new AttachmentFileResponse(attachment.Data, attachment.ContentType, attachment.OriginalFileName);

        return response;
    }
}
