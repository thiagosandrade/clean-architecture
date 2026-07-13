using Application.Abstractions.Messaging;
using Application.Todos.GetByDescription;
using SharedKernel;

namespace Application.Todos.Search;

public sealed record SearchTodosQuery(string Searchtext, Guid UserId, Paginated? Pagination, Sorted? Sorting) 
    : IQuery<PagedResponse<SearchTodoResponse>>;
