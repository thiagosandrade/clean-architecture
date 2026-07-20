using Domain.Activities;

namespace Application.Todos.Activities.Log;

public interface ITodoActivityService
{
    Task LogAsync(
        Guid todoId,
        TaskActivityType activityType,
        string description,
        Guid userId,
        object? metadata = null,
        CancellationToken cancellationToken = default);
}
