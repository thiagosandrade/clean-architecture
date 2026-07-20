using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.OpenAI.Enrichment;
using Domain.Activities;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Breakdown;

internal sealed partial class TaskBreakdownCommandHandler(
    IApplicationDbContext context,
    ISubTaskEnrichmentService subTaskEnrichmentService,
    IUserContext userContext)
    : ICommandHandler<TaskBreakdownCommand, BreakdownResponse>
{
    public async Task<Result<BreakdownResponse>> Handle(TaskBreakdownCommand command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<BreakdownResponse>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<BreakdownResponse>(UserErrors.NotFound(command.UserId));
        }

        TodoItem todoItem = await context.TodoItems
            .Include(x => x.SubItems)
            .FirstAsync(x => x.Id == command.TodoId, cancellationToken);

        IReadOnlyCollection<string> subTasks = await subTaskEnrichmentService.GenerateSubTasksAsync(todoItem.Description, command.Strategy, command.Complexity, cancellationToken);

        todoItem.Raise(new TodoActivityLogRequestedDomainEvent(todoItem.Id, TaskActivityType.BreakdownGenerated, "BreakdownGenerated", todoItem.UserId));

        return new BreakdownResponse(subTasks);
    }
}
