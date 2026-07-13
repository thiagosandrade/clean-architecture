namespace Application.Todos.GetById;

public class TaskDependencyResponse
{
    public Guid TodoItemId { get; set; }
    public Guid DependsOnTodoItemId { get; set; }
    public string Description { get; set; } = string.Empty;
}
