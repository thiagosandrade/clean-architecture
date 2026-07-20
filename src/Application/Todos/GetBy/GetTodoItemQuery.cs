using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.GetBy;

public sealed record GetTodoItemQuery(Guid UserId, string PartialDescription) 
    : IQuery<List<GetTodoItemQueryResponse>>;


