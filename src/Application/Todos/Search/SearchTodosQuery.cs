using Application.Abstractions.Messaging;
using Application.Todos.Get;

namespace Application.Todos.Search;

public sealed record SearchTodosQuery(string searchtext) : IQuery<List<SearchTodoResponse>>;
