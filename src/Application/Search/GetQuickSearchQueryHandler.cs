using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Search;

internal sealed class GetQuickSearchQueryHandler(
    IApplicationDbContext context,
    IUserContext userContext)
    : IQueryHandler<QuickSearchQuery, QuickSearchResponse>
{
    public async Task<Result<QuickSearchResponse>> Handle(
        QuickSearchQuery query,
        CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<QuickSearchResponse>(UserErrors.Unauthorized());
        }

        string pattern = $"%{query.Text}%";
        int limit = query.Limit <= 0 ? 5 : query.Limit;

        List<TodoItemSearch> todoItem = await context.TodoItems
            .AsNoTracking()
            .Where(t =>
                t.UserId == query.UserId &&
                EF.Functions.ILike(t.Description, pattern))
            .OrderByDescending(t => t.UpdatedOn ?? t.CreatedOn)
            .Take(limit)
            .Select(t => new TodoItemSearch
            {
                Id = t.Id,
                Description = t.Description,
                Priority = (int)t.Priority,
                DueDate = t.DueDate,
                Completed = t.IsCompleted
            })
            .ToListAsync(cancellationToken);

        List<TodoSubItemSearch> subtasks = await context.TodoSubItems
            .AsNoTracking()
            .Where(s =>
                s.TodoItem.UserId == query.UserId &&
                EF.Functions.ILike(s.Description, pattern))
            .OrderByDescending(s => s.TodoItem.UpdatedOn ?? s.TodoItem.CreatedOn)
            .Take(limit)
            .Select(s => new TodoSubItemSearch
            {
                Id = s.Id,
                Description = s.Description,
                Completed = s.IsCompleted,
                TaskId = s.TodoItemId,
                TaskDescription = s.TodoItem.Description
            })
            .ToListAsync(cancellationToken);

        List<TodoItemAttachmentSearch> attachments = await context.TodoAttachments
            .AsNoTracking()
            .Where(a =>
                a.TodoItem.UserId == query.UserId &&
                EF.Functions.ILike(a.OriginalFileName, pattern))
            .OrderByDescending(a => a.TodoItem.UpdatedOn ?? a.TodoItem.CreatedOn)
            .Take(limit)
            .Select(a => new TodoItemAttachmentSearch
            {
                Id = a.Id,
                OriginalFileName = a.OriginalFileName,
                ContentType = a.ContentType,
                Size = a.Size,
                TaskId = a.TodoItemId,
                TaskDescription = a.TodoItem.Description
            })
            .ToListAsync(cancellationToken);

        List<UserItemSearch> users = await context.Users
            .AsNoTracking()
            .Where(u =>
                EF.Functions.ILike(u.Email, pattern) ||
                EF.Functions.ILike(u.FirstName, pattern) ||
                EF.Functions.ILike(u.LastName, pattern))
            .OrderBy(u => u.Email)
            .Take(limit)
            .Select(u => new UserItemSearch
            {
                Id = u.Id,
                DisplayName = $"{u.FirstName} {u.LastName}",
                Email = u.Email
            })
            .ToListAsync(cancellationToken);

        return new QuickSearchResponse
        {
            Tasks = todoItem,
            Subtasks = subtasks,
            Attachments = attachments,
            Users = users
        };
    }
}
