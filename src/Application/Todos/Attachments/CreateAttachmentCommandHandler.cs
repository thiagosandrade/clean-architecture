using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Attachments;

internal sealed class CreateAttachmentCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider provider,
    IUserContext userContext)
    : ICommandHandler<CreateAttachmentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateAttachmentCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId != userContext.UserId)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }

        TodoItem? todo = await context.TodoItems
            .Include(x => x.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId, cancellationToken);

        if (todo is null)
        {
            return Result.Failure<Guid>(TodoItemErrors.NotFound(command.TodoId));
        }

        var attachment = new TaskAttachment
        {
            TodoItemId = command.TodoId,
            OriginalFileName = command.OriginalFileName,
            StoredFileName = command.StoredFileName,
            ContentType = command.ContentType,
            Size = command.Size,
            Data = command.Data,
            CreatedOn = provider.UtcNow,
            CreatedBy = command.UserId
        };

        todo.AddAttachment(attachment);

        todo.Raise(new TaskActivityLogRequestedDomainEvent(todo.Id, TaskActivityType.AttachmentAdded, "Attachment Added", userContext.UserId));

        await context.SaveChangesAsync(cancellationToken);

        return attachment.Id;
    }
}
