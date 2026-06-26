using Application.Abstractions.Messaging;
using Domain.Todos;

namespace Application.Todos.Breakdown;

public sealed class TaskBreakdownCommand : ICommand
{
    public Guid UserId { get; set; }
    public Guid TodoId { get; set; }
}
