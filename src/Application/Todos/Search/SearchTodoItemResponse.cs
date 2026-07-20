using Application.Todos.Get;

namespace Application.Todos.Search;

public sealed class SearchTodoItemResponse : TodoItemResponse
{
    public double Similarity { get; internal set; }
}

public sealed class SearchTodoSubItemResponse : TodoSubItemResponse
{

}
