using Domain.Todos;

namespace Application.Todos.GetById;

public sealed class TodoResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? CompletedOn { get; set; }
    public IEnumerable<string> Labels { get; set; }
    public IEnumerable<TodoSubItemResponse> SubItems { get; set; } = [];
    public IEnumerable<TaskDependencyResponse> Dependencies { get; set; } = [];

    public IEnumerable<string> Categories { get; set; }
}
