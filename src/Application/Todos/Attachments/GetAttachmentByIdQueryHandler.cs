using Microsoft.EntityFrameworkCore;
using Domain.Users;
using Domain.Todos;
using SharedKernel.Authentication;
using SharedKernel.Abstractions.Data;
using SharedKernel.Abstractions.Messaging;
using Domain;

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
