using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.Messaging;

namespace Application.Todos.Dependency;

public sealed record EditTodoDependenciesCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }

    public Guid TodoId { get; init; }

    public IReadOnlyCollection<Guid> Dependencies { get; init; } = [];
}
