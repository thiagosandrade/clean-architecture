using Application.Common.Interfaces;
using Application.OpenAI.Enrichment;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.Activities;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Rewrite;

internal sealed class SubtaskRewriteCommandHander(
    IApplicationDbContext context,
    IRewriteEnrichmentService subtaskRewriteService,
    IRabbitMqPublisher publisher,
    IUserContext userContext)
    : ICommandHandler<SubtaskRewriteCommand, SubtaskRewriteResponse>
{
    public async Task<Result<SubtaskRewriteResponse>> Handle(SubtaskRewriteCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<SubtaskRewriteResponse>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<SubtaskRewriteResponse>(UserErrors.NotFound(command.UserId));
        }

        TodoItem? todoItem = await context.TodoItems
            .SingleOrDefaultAsync(t => t.Id == command.TodoId && t.UserId == userContext.UserId, cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<SubtaskRewriteResponse>(TodoItemErrors.NotFound(command.TodoId));
        }

        SubtaskRewriteResponse response = await subtaskRewriteService.RewriteAsync(command.Description, command.Style, cancellationToken);

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(command.TodoId, TaskActivityType.DescriptionRewritten, "Description Rewritten but not saved yet", user.Id));

        await publisher.PublishAsync(new TodoUpdatedIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);

        return Result.Success<SubtaskRewriteResponse>(response);
    }
}
