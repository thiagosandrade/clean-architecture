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

namespace Application.Todos.Breakdown;

internal sealed partial class TaskBreakdownCommandHandler(
    IApplicationDbContext context,
    ISubTaskEnrichmentService subTaskEnrichmentService,
    IRabbitMqPublisher publisher,
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

        await publisher.PublishAsync(new TodoBreakdownIntegrationEvent(todoItem.Id, todoItem.UserId, todoItem.Description), cancellationToken);

        return new BreakdownResponse(subTasks);
    }
}
