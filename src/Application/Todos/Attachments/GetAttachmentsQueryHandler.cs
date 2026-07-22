using Microsoft.EntityFrameworkCore;
using Domain.Users;
using Domain.Todos;
using SharedKernel.Abstractions.Messaging;
using Domain.API;
using Application.Common.Interfaces;

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

        List<TodoAttachment> attachments = await context.TodoAttachments
            .Where(a => a.TodoItemId == query.TodoId)
            .OrderByDescending(a => a.CreatedOn)
            .ToListAsync(cancellationToken);

        var response = new AttachmentsResponse(attachments.Select(a => new AttachmentResponse(a)));

        return response;
    }
}
