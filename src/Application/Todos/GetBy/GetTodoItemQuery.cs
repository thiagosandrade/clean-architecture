using Application.Abstractions.Messaging;
using Domain.Todos;
using SharedKernel;

namespace Application.Todos.GetBy;

public sealed record GetTodoItemQuery(Guid UserId, string PartialDescription) 
    : IQuery<List<GetTodoItemQueryResponse>>;


