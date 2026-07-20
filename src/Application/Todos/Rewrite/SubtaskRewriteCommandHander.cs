using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.OpenAI.Enrichment;
using Application.Todos.Breakdown;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Rewrite;

internal sealed class SubtaskRewriteCommandHander(
    IApplicationDbContext context,
    IRewriteEnrichmentService subtaskRewriteService,
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

        return Result.Success<SubtaskRewriteResponse>(response);
    }
}
