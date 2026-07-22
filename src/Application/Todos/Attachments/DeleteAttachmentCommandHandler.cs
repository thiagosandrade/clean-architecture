using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.Activities;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Attachments;

internal sealed class DeleteAttachmentCommandHandler(
    IApplicationDbContext context,
    IRabbitMqPublisher publisher,
    IUserContext userContext)
    : ICommandHandler<DeleteAttachmentCommand>
{
    public async Task<Result> Handle(DeleteAttachmentCommand command, CancellationToken cancellationToken)
    {
        if (command.UserId != userContext.UserId)
        {
            return Result.Failure(UserErrors.Unauthorized());
        }

        TodoAttachment? attachment = await context.TodoAttachments
            .SingleOrDefaultAsync(a => a.Id == command.AttachmentId && a.TodoItemId == command.TodoId, cancellationToken);

        if (attachment is null)
        {
            return Result.Failure(TodoItemErrors.AttachmentNotFound(command.AttachmentId));
        }

        TodoItem? todo = await context.TodoItems
            .Include(x => x.Attachments)
            .SingleOrDefaultAsync(t => t.Id == command.TodoId, cancellationToken);

        if (todo is null)
        {
            return Result.Failure<Guid>(TodoItemErrors.NotFound(command.TodoId));
        }

        todo.RemoveAttachment(attachment.Id);

        todo.Raise(new TodoActivityLogRequestedDomainEvent(todo.Id, TaskActivityType.AttachmentDeleted, "Attachment Deleted", userContext.UserId));

        await context.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(new TodoAttachmentIntegrationEvent(todo.Id, todo.UserId, todo.Description), cancellationToken);

        return Result.Success();
    }
}
