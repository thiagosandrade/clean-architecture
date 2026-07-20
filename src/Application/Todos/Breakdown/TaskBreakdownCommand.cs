using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Breakdown;

public sealed class TaskBreakdownCommand : ICommand<BreakdownResponse>
{
    public Guid UserId { get; set; }
    public Guid TodoId { get; set; }
    public BreakdownComplexity Complexity { get; set; }
    public BreakdownStrategy Strategy { get; set; }
}
