using Application.Common.Interfaces;
using Domain.Activities;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;
using System.Linq.Dynamic.Core;

namespace Application.Todos.Activities.Get;

internal sealed class GetTodoActivitiesQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodoActivitiesQuery, GetTodoActivityResponse>
{
    public async Task<Result<GetTodoActivityResponse>> Handle(GetTodoActivitiesQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<GetTodoActivityResponse>(UserErrors.Unauthorized());
        }

        List<TodoActivity> resultActivities = await context
            .TodoActivities
            .Include(x => x.User)
            .Where(x => x.TodoItemId == query.TodoId)
            .OrderByDescending(x => x.CreatedOn)
            .ToListAsync(cancellationToken);

        var result = new GetTodoActivityResponse(resultActivities);

        return result;
    }
}
