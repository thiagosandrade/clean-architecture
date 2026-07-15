using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Abstractions.Authentication;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Domain.Users;
using Domain.Todos;

namespace Application.Todos.Attachments;

internal sealed class GetAttachmentsQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<GetAttachmentsQuery, AttachmentsResponse>
{
    public async Task<Result<AttachmentsResponse>> Handle(GetAttachmentsQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<AttachmentsResponse>(UserErrors.Unauthorized());
        }

        List<TaskAttachment> attachments = await context.TaskAttachments
            .Where(a => a.TodoItemId == query.TodoId)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync(cancellationToken);

        var response = new AttachmentsResponse(attachments.Select(a => new AttachmentResponse(a)));

        return response;
    }
}
