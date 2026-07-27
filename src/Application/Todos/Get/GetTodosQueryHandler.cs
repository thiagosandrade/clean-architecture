using Application.Common.Interfaces;
using Application.Elastic.Documents;
using Application.Elastic.Services;
using Application.OpenAI.Embeddings;
using Domain.API;
using Domain.Users;
using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Get;

internal sealed class GetTodosQueryHandler(IElasticTodoSearchService elasticSearchService, IUserContext userContext)
    : IQueryHandler<GetTodosQuery, PagedResponse<TodoItemResponse>>
{
    public async Task<Result<PagedResponse<TodoItemResponse>>> Handle(GetTodosQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<PagedResponse<TodoItemResponse>>(UserErrors.Unauthorized());
        }

        PagedResponse<TodoDocument> result =
            await elasticSearchService.SearchTodosAsync(
                query.UserId,
                query.Filter,
                query.Sorting,
                query.Pagination,
                cancellationToken);

        var todos = result.Items
            .Select(todo => new TodoItemResponse
            {
                Id = todo.Id,
                UserId = todo.UserId,
                Description = todo.Description,
                DueDate = todo.DueDate,
                Labels = todo.Labels,
                Categories = todo.Categories,
                IsCompleted = todo.IsCompleted,
                CreatedOn = todo.CreatedOn,
                CompletedOn = todo.CompletedOn,
                Priority = (Domain.Todos.Priority)todo.Priority,

                SubItems = todo.Subtasks.Select(subItem =>
                    new TodoSubItemResponse
                    {
                        TodoItemId = subItem.SubItemId,
                        Id = subItem.Id,
                        Description = subItem.Description,
                        IsCompleted = subItem.IsCompleted,
                        Order = subItem.Order,
                        CreatedOn = subItem.CreatedOn,
                        CompletedOn = subItem.CompletedOn
                    })
            })
            .ToList();

        return new PagedResponse<TodoItemResponse>(
            todos,
            result.Total);
    }
}
