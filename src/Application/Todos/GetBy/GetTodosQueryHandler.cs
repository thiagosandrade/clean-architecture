using System.Linq.Dynamic.Core;
using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.GetBy;

internal sealed class GetTaskQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTaskQuery, List<GetTaskQueryResponse>>
{
    public async Task<Result<List<GetTaskQueryResponse>>> Handle(GetTaskQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<List<GetTaskQueryResponse>>(UserErrors.Unauthorized());
        }

        IQueryable<GetTaskQueryResponse> todos = context.TodoItems
            .AsNoTracking()
            .Where(t =>
                t.UserId == query.UserId &&
                EF.Functions.ILike(
                    t.Description,
                    $"%{query.PartialDescription}%"
                )
            )
            .OrderBy(x => x.Description)
            .Take(20)
            .Select(todoItem => new GetTaskQueryResponse
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
            });

        List<GetTaskQueryResponse> result = await todos.ToListAsync(cancellationToken);

        return result;
    }
}
