using Application.Abstractions.Messaging;
using Domain.Todos;
using SharedKernel;

namespace Application.Todos.Activities.Get;

public sealed record GetTaskActivitiesQuery(Guid TodoId, Guid UserId) 
    : IQuery<GetTaskActivityResponse>;
