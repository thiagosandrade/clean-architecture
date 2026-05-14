using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Todos.Get;

public sealed record GetTodosQuery(Guid UserId, Paginated? Pagination, Sorted? Sorting) 
    : IQuery<PagedResponse<TodoResponse>>;
