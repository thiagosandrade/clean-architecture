using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.EditSubItem;

public sealed class EditTodoSubItemsCommand : ICommand<Guid>
{
    public Guid UserId { get; set; }

    public Guid TodoId { get; set; }

    public IEnumerable<TodoSubItemCommand> TodoSubItems { get; set; } = [];
}

public sealed class TodoSubItemCommand
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int Order { get; set; }
}
