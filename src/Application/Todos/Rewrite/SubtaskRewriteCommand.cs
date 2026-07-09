using Application.Abstractions.Messaging;
using Domain.Todos;

namespace Application.Todos.Rewrite;

public sealed record SubtaskRewriteCommand : ICommand<SubtaskRewriteResponse>
{
    public Guid UserId { get; init; }

    public Guid TodoId { get; init; }

    public string Description { get; init; } = string.Empty;

    public RewriteStyle Style { get; init; }
}
