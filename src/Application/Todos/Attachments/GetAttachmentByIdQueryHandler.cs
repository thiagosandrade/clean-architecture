using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Domain.Users;
using Domain.Todos;

namespace Application.Todos.Attachments;

internal sealed class GetAttachmentByIdQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetAttachmentByIdQuery, AttachmentResponse>
{
    public async Task<Result<AttachmentResponse>> Handle(GetAttachmentByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<AttachmentResponse>(UserErrors.Unauthorized());
        }

        TodoAttachment? attachment = await context.TodoAttachments
            .SingleOrDefaultAsync(a => a.Id == query.AttachmentId && a.TodoItemId == query.TodoId, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure<AttachmentResponse>(TodoItemErrors.AttachmentNotFound(query.AttachmentId));
        }

        return new AttachmentResponse(attachment);
    }
}
