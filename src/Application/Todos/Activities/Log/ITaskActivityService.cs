using Domain.Activities;

namespace Application.Todos.Activities.Log;

public interface ITaskActivityService
{
    Task LogAsync(
        Guid todoId,
        TaskActivityType activityType,
        string description,
        Guid userId,
        object? metadata = null,
        CancellationToken cancellationToken = default);
}
