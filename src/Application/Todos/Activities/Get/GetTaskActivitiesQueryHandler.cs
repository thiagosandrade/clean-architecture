using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Activities;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Linq.Dynamic.Core;

namespace Application.Todos.Activities.Get;

internal sealed class GetTaskActivitiesQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTaskActivitiesQuery, GetTaskActivityResponse>
{
    public async Task<Result<GetTaskActivityResponse>> Handle(GetTaskActivitiesQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<GetTaskActivityResponse>(UserErrors.Unauthorized());
        }

        List<TaskActivity> resultActivities = await context
            .TaskActivities
            .Include(x => x.User)
            .Where(x => x.TodoItemId == query.TodoId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        var result = new GetTaskActivityResponse(resultActivities);

        return result;
    }
}
