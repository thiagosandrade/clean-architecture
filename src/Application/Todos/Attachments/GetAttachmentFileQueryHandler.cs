using Microsoft.EntityFrameworkCore;
using Domain.Users;
using Domain.Todos;
using SharedKernel.Authentication;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;
using Domain;

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

        TodoAttachment? attachment = await context.TodoAttachments
            .SingleOrDefaultAsync(a => a.Id == query.AttachmentId && a.TodoItemId == query.TodoId, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure<AttachmentFileResponse>(TodoItemErrors.AttachmentNotFound(query.AttachmentId));
        }

        var response = new AttachmentFileResponse(attachment.Data, attachment.ContentType, attachment.OriginalFileName);

        return response;
    }
}
