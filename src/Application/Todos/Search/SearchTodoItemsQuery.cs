using Domain;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Search;

public sealed record SearchTodoItemsQuery(string Searchtext, Guid UserId, Paginated? Pagination, Sorted? Sorting) 
    : IQuery<PagedResponse<SearchTodoItemResponse>>;
