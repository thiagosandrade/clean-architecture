using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Activities.Get;

public sealed record GetTodoActivitiesQuery(Guid TodoId, Guid UserId) 
    : IQuery<GetTodoActivityResponse>;
