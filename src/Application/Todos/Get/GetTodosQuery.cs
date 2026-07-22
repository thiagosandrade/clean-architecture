using Domain.API;
using Domain.Todos;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId, Paginated? Pagination, Sorted? Sorting, TodoFilter? Filter) 
    : IQuery<PagedResponse<TodoItemResponse>>;

public sealed record TodoFilter
{
    public DateOnly? DueDateFrom { get; init; }
    
    public DateOnly? DueDateTo { get; init; }

    public Priority? Priority { get; init; }

    public bool? IsCompleted { get; init; }

    public IReadOnlyCollection<string>? Categories { get; init; }

    public IReadOnlyCollection<string>? Labels { get; init; }
}
