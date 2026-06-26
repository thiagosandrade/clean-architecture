using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.OpenAI.Enrichment;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Breakdown;

internal sealed class TaskBreakdownCommandHandler(
    IApplicationDbContext context,
    ISubTaskEnrichmentService subTaskEnrichmentService,
    IUserContext userContext)
    : ICommandHandler<TaskBreakdownCommand>
{
    public async Task<Result> Handle(TaskBreakdownCommand  command, CancellationToken cancellationToken)
    {
        if (userContext.UserId != command.UserId)
        {
            return Result.Failure<Guid>(UserErrors.Unauthorized());
        }

        User? user = await context.Users.AsNoTracking()
            .SingleOrDefaultAsync(u => u.Id == command.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        TodoItem todo = await context.TodoItems
            .Include(x => x.SubItems)
            .FirstAsync(x => x.Id == command.TodoId, cancellationToken);

        if (todo.SubItems.Any())
        {
            return Result.Failure(SubTaskErrors.AlreadyGenerated(command.TodoId));
        }

        IReadOnlyCollection<string> subTasks = await subTaskEnrichmentService.GenerateSubTasksAsync(todo.Description, cancellationToken);

        todo.AddSubItems([.. subTasks
            .Select((description, index) => new TodoSubItem
            {
                Description = description,
                TodoItemId = todo.Id,
                Order = index
            })]);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
