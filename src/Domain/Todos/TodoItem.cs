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
    
    public IEnumerable<TaskActivity> TaskActivities { get; set; } = [];

    private readonly List<TodoSubItem> _subItems = [];
    public IEnumerable<TodoSubItem> SubItems => _subItems;


    private readonly List<TaskDependency> _dependencies = [];
    public IEnumerable<TaskDependency> Dependencies => _dependencies;

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

    public void AddDependency(Guid dependsOnId)
    {
        if (_dependencies.Any(x =>
            x.DependsOnTodoItemId == dependsOnId))
        {
            return;
        }

        _dependencies.Add(
            new TaskDependency(Id, dependsOnId));
    }

    public void RemoveDependency(Guid dependsOnId)
    {
        TaskDependency? dependency =
            _dependencies.FirstOrDefault(
                x => x.DependsOnTodoItemId == dependsOnId);

        if (dependency != null)
        {
            _dependencies.Remove(dependency);
        }
    }
}
