using Application.Todos.Get;

namespace Application.Todos.Search;

public sealed class SearchTodoResponse : TodoResponse
{
    public double Similarity { get; internal set; }
}

public sealed class SearchTodoSubItemResponse : TodoSubtaskResponse
{

}
