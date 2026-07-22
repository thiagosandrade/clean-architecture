using System.Text.Json;
using Application.Common.Interfaces;
using Domain.Activities;

namespace Application.Todos.Activities.Log;

public class TodoActivityService(IApplicationDbContext applicationDbContext, IDateTimeProvider dateTimeProvider) : ITodoActivityService
{
    public async Task LogAsync(
        Guid todoId,
        TaskActivityType activityType,
        string description,
        Guid userId,
        object? metadata = null,
        CancellationToken cancellationToken = default)
    {
        var activity = new TodoActivity
        {
            TodoItemId = todoId,

            ActivityType = activityType,

            Description = description,

            UserId = userId,

            CreatedOn = dateTimeProvider.UtcNow,

            Metadata = metadata == null
                ? null
                : JsonSerializer.Serialize(metadata)
        };

        applicationDbContext.TodoActivities.Add(activity);

        await applicationDbContext.SaveChangesAsync(cancellationToken);
    }
}
