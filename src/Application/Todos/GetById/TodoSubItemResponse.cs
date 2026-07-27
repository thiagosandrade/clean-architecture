namespace Application.Todos.GetById;

public class TodoSubItemResponse
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public int Order { get; set; }
}
