using Application.Abstractions.Messaging;
using Domain.Todos;
using SharedKernel;

namespace Application.Todos.Activities.Get;

public sealed record GetTodoActivitiesQuery(Guid TodoId, Guid UserId) 
    : IQuery<GetTodoActivityResponse>;
