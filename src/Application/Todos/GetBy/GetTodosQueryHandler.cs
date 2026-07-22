using System.Linq.Dynamic.Core;
using Application.Common.Interfaces;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.GetBy;

internal sealed class GetTaskQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<GetTodoItemQuery, List<GetTodoItemQueryResponse>>
{
    public async Task<Result<List<GetTodoItemQueryResponse>>> Handle(GetTodoItemQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<List<GetTodoItemQueryResponse>>(UserErrors.Unauthorized());
        }

        IQueryable<GetTodoItemQueryResponse> todos = context.TodoItems
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
            .Select(todoItem => new GetTodoItemQueryResponse
            {
                Id = todoItem.Id,
                Description = todoItem.Description,
            });

        List<GetTodoItemQueryResponse> result = await todos.ToListAsync(cancellationToken);

        return result;
    }
}
