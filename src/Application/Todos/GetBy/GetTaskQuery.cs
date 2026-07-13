using Application.Abstractions.Messaging;
using Domain.Todos;
using SharedKernel;

namespace Application.Todos.GetBy;

public sealed record GetTaskQuery(Guid UserId, string PartialDescription) 
    : IQuery<List<GetTaskQueryResponse>>;


