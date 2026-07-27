using Domain.Activities;
using Pgvector;

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
    public DateTime? CompletedOn { get; set; }
    public Priority Priority { get; set; }
    public Vector? Embedding { get; set; } = default!;


    private readonly List<TodoActivity> _activities = [];
    public IEnumerable<TodoActivity> TaskActivities => _activities;


    private readonly List<TodoSubItem> _subItems = [];
    public IEnumerable<TodoSubItem> SubItems => _subItems;


    private readonly List<TodoDependency> _dependencies = [];
    public IEnumerable<TodoDependency> Dependencies => _dependencies;


    private readonly List<TodoAttachment> _attachments = [];
    public IEnumerable<TodoAttachment> Attachments => _attachments;

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
            new TodoDependency(Id, dependsOnId));
    }

    public void RemoveDependency(Guid dependsOnId)
    {
        TodoDependency? dependency =
            _dependencies.FirstOrDefault(
                x => x.DependsOnTodoItemId == dependsOnId);

        if (dependency != null)
        {
            _dependencies.Remove(dependency);
        }
    }

    public void AddAttachment(TodoAttachment taskAttachment)
    {
        _attachments.Add(taskAttachment);
    }

    public void RemoveAttachment(Guid taskAttachmentId)
    {
        TodoAttachment? attachment =
            _attachments.FirstOrDefault(
                x => x.Id == taskAttachmentId);

        if (attachment != null)
        {
            _attachments.Remove(attachment);
        }
    }
}
