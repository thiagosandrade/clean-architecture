using Domain.Activities;
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
    public IEnumerable<TodoSubItem> SubItems => _subItems;
    public IEnumerable<TaskActivity> TaskActivities { get; set; } = [];

    private readonly List<TodoSubItem> _subItems = [];

    public void AddSubItems(IEnumerable<TodoSubItem> subItems)
    {
        _subItems.AddRange([.. SubItems, .. subItems]);
    }

    public void AddSubItem(TodoSubItem item)
    {
        _subItems.Add(item);
    }

    public void RemoveSubItem(Guid id)
    {
        TodoSubItem? item = _subItems
            .FirstOrDefault(x => x.Id == id);

        if (item is not null)
        {
            _subItems.Remove(item);
        }
    }

    public TodoSubItem? GetSubItem(Guid id)
    {
        return _subItems.FirstOrDefault(x => x.Id == id);
    }
}
