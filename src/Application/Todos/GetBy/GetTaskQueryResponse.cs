using Application.Todos.GetById;
using Domain.Todos;

namespace Application.Todos.GetBy;

public class GetTaskQueryResponse
{
    public Guid Id { get; set; }
    public string Description { get; set; }
}
