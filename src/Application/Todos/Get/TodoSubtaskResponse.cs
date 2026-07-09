namespace Application.Todos.Get;

public class TodoSubtaskResponse
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public int Order { get; set; }
}
