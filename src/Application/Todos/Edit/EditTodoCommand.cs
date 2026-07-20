using Domain.Todos;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Edit;

public sealed class EditTodoCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
