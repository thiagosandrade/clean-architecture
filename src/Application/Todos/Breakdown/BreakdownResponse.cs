namespace Application.Todos.Breakdown;

public class BreakdownResponse
{
    public BreakdownResponse(IReadOnlyCollection<string> subtasks)
    {
        Subtasks = subtasks;
    }

    public IEnumerable<string> Subtasks { get; set; }
};
