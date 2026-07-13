using SharedKernel;

namespace Domain.Todos;

public static class TodoItemErrors
{
    public static Error AlreadyCompleted(Guid todoItemId) => Error.Problem(
        "TodoItemErrors.AlreadyCompleted",
        $"The todo item with Id = '{todoItemId}' is already completed.");

    public static Error NotFound(Guid todoItemId) => Error.NotFound(
        "TodoItemErrors.NotFound",
        $"The to-do item with the Id = '{todoItemId}' was not found");
}

public static class SubTaskErrors
{
    public static Error AlreadyGenerated(Guid todoItemId) => Error.Problem(
        "SubTaskErrors.TaskBreakdownAlreadyGenerated",
        $"Substasks for TodoItem with Id = '{todoItemId}' were already generated.");
}

public static class DependencyErrors
{
    public static Error CannotDependOnItself(Guid todoItemId) => Error.Problem(
        "DependencyErrors.InvalidDependency",
        $"Invalid dependency for TodoItem with Id = '{todoItemId}' because they can't depend on itself.");

    public static Error CannotDependOnAnotherUserTask(Guid todoItemId, Guid dependencyItemId) => Error.Problem(
        "DependencyErrors.CannotDependOnAnotherUserTask",
        $"Invalid dependency for TodoItem with Id = '{todoItemId}' because they can't depend on another user's task with id = '{dependencyItemId}'.");

    public static Error DependencyNotFound(Guid todoItemId, Guid dependencyItemId) => Error.Problem(
        "DependencyErrors.DependencyNotFound",
        $"Invalid dependency for TodoItem with Id = '{todoItemId}' because the item with id = '{dependencyItemId}' doesn't exist.");
}
