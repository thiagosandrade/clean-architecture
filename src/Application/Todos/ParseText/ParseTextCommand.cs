using Application.OpenAI.Parser;
using Domain.Todos;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.ParseText;

public sealed class ParseTextCommand : ICommand<TodoExtractorResponse>
{
    public Guid UserId { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public List<string> Labels { get; set; } = [];
    public Priority Priority { get; set; }
}
