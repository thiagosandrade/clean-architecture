using SharedKernel;

namespace Domain.Todos;

public static class TodoItemErrors
{
    public static Error AlreadyCompleted(Guid todoItemId) => Error.Problem(
        "TodoItems.AlreadyCompleted",
        $"The todo item with Id = '{todoItemId}' is already completed.");

    public static Error NotFound(Guid todoItemId) => Error.NotFound(
        "TodoItems.NotFound",
        $"The to-do item with the Id = '{todoItemId}' was not found");
}

public static class SubTaskErrors
{
    public static Error AlreadyGenerated(Guid todoItemId) => Error.Problem(
        "TodoSubTask.TaskBreakdownAlreadyGenerated",
        $"Substasks for TodoItem with Id = '{todoItemId}' were already generated.");
}
