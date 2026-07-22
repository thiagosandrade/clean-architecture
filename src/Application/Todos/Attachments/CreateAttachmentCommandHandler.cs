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

internal sealed class CreateAttachmentCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider provider,
    IRabbitMqPublisher publisher,
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

        var attachment = new TodoAttachment
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

        todo.Raise(new TodoActivityLogRequestedDomainEvent(todo.Id, TaskActivityType.AttachmentAdded, "Attachment Added", userContext.UserId));

        await context.SaveChangesAsync(cancellationToken);

        await publisher.PublishAsync(new TodoAttachmentIntegrationEvent(todo.Id, todo.UserId, todo.Description),cancellationToken);

        return attachment.Id;
    }
}
