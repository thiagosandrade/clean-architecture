using Pgvector;
using SharedKernel;

namespace Domain.Todos;

public sealed class TodoItem : Entity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public IEnumerable<string> Labels { get; set; } = [];
    public IEnumerable<string> Categories { get; set; } = [];
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Priority Priority { get; set; }
    public Vector? Embedding { get; set; } = default!;
    public IEnumerable<TodoSubItem> SubItems { get; set; } = [];

    public void AddSubItems(IEnumerable<TodoSubItem> subItems)
    {
        SubItems = [.. SubItems, .. subItems];
    }
}
