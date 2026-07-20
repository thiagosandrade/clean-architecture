using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Search;

internal sealed class GetSearchQueryHandler(IApplicationDbContext context, IUserContext userContext)
    : IQueryHandler<SearchQuery, SearchResponse>
{
    public async Task<Result<SearchResponse>> Handle(SearchQuery query, CancellationToken cancellationToken)
    {
        if (query.UserId != userContext.UserId)
        {
            return Result.Failure<SearchResponse>(UserErrors.Unauthorized());
        }

        string pattern = $"%{query.Text}%";
        int skip = (Math.Max(query.Page, 1) - 1) * Math.Max(query.PageSize, 1);
        int take = Math.Max(query.PageSize, 1);

        List<TaskSearchItem> tasksQuery = await HandleTasks(query, pattern, skip, take, cancellationToken);

        List<SubtaskSearchItem> subtasksQuery = await HandleSubTasks(query, pattern, skip, take, cancellationToken);

        List<AttachmentSearchItem> attachmentsQuery = await HandleTaskAttachments(query, pattern, skip, take, cancellationToken);

        List<UserSearchItem> usersQuery = await HandleUsers(pattern, skip, take, cancellationToken);

        var response = new SearchResponse
        {
            Tasks = tasksQuery,
            Subtasks = subtasksQuery,
            Attachments = attachmentsQuery,
            Users = usersQuery
        };

        return response;
    }

    private async Task<List<UserSearchItem>> HandleUsers(string pattern, int skip, int take, CancellationToken cancellationToken)
    {
        return await context.Users
                    .AsNoTracking()
                    .Where(u =>
                        EF.Functions.ILike(u.Email, pattern) ||
                        EF.Functions.ILike(u.FirstName, pattern) ||
                        EF.Functions.ILike(u.LastName, pattern))
                    .OrderBy(u => u.Email)
                    .Skip(skip)
                    .Take(take)
                    .Select(u => new UserSearchItem
                    {
                        Id = u.Id,
                        DisplayName = $"{u.FirstName} {u.LastName}",
                        Email = u.Email
                    })
                    .ToListAsync(cancellationToken);
    }

    private async Task<List<AttachmentSearchItem>> HandleTaskAttachments(SearchQuery query, string pattern, int skip, int take, CancellationToken cancellationToken)
    {
        return await context.TodoAttachments
                    .Include(x => x.TodoItem)
                    .AsNoTracking()
                    .Where(a =>
                        a.TodoItem.UserId == query.UserId &&
                        EF.Functions.ILike(a.OriginalFileName, pattern))
                    .OrderByDescending(a => a.UpdatedOn ?? a.CreatedOn)
                    .Skip(skip)
                    .Take(take)
                    .Select(a => new AttachmentSearchItem
                    {
                        Id = a.Id,
                        OriginalFileName = a.OriginalFileName,
                        ContentType = a.ContentType,
                        Size = a.Size,
                        TaskId = a.TodoItemId,
                        TaskDescription = a.TodoItem.Description
                    })
                    .ToListAsync(cancellationToken);
    }

    private async Task<List<SubtaskSearchItem>> HandleSubTasks(SearchQuery query, string pattern, int skip, int take, CancellationToken cancellationToken)
    {
        return await context.TodoSubItems
                    .Include(x => x.TodoItem)
                    .AsNoTracking()
                    .Where(s =>
                        s.TodoItem.UserId == query.UserId &&
                        EF.Functions.ILike(s.Description, pattern))
                    .OrderByDescending(s => s.UpdatedOn ?? s.CreatedOn)
                    .Skip(skip)
                    .Take(take)
                    .Select(s => new SubtaskSearchItem
                    {
                        Id = s.Id,
                        Description = s.Description,
                        Completed = s.IsCompleted,
                        TaskId = s.TodoItemId,
                        TaskDescription = s.Description
                    })
                    .ToListAsync(cancellationToken);
    }

    private async Task<List<TaskSearchItem>> HandleTasks(SearchQuery query, string pattern, int skip, int take, CancellationToken cancellationToken)
    {
        return await context.TodoItems
                    .AsNoTracking()
                    .Where(t => t.UserId == query.UserId && EF.Functions.ILike(t.Description, pattern))
                    .OrderByDescending(t => t.UpdatedOn ?? t.CreatedOn)
                    .Skip(skip)
                    .Take(take)
                    .Select(t => new TaskSearchItem
                    {
                        Id = t.Id,
                        Description = t.Description,
                        Priority = (int)t.Priority,
                        DueDate = t.DueDate,
                        Completed = t.IsCompleted
                    })
                    .ToListAsync(cancellationToken);
    }
}
