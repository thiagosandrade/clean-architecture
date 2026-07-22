using Application.Common.Interfaces;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Search;

internal sealed class GetSearchDetailQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetSearchDetailQuery, SearchDetailResponse>
{
    private const string TODOITEM = "TODOITEM";
    private const string TODOSUBITEM = "TODOSUBITEM";
    private const string ATTACHMENT = "ATTACHMENT";
    private const string USER = "USER";

    public async Task<Result<SearchDetailResponse>> Handle(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        string type = (query.Type ?? string.Empty).Trim().ToUpperInvariant();

        if (type == TODOITEM)
        {
            return await HandleTodoItem(query, cancellationToken);
        }

        if (type == TODOSUBITEM)
        {
            return await HandleSubItem(query, cancellationToken);
        }

        if (type == ATTACHMENT)
        {
            return await HandleAttachment(query, cancellationToken);
        }

        if (type == USER)
        {
            return await HandleUser(query, cancellationToken);
        }

        return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
    }

    private async Task<Result<SearchDetailResponse>> HandleTodoItem(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        TodoItem? todo = await context.TodoItems
            .AsNoTracking()
            .Include(t => t.SubItems)
            .Include(t => t.Attachments)
            .Where(t => t.Id == query.Id && t.UserId == query.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (todo is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        var owner = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == todo.UserId)
            .Select(u => new { u.FirstName, u.LastName })
            .SingleOrDefaultAsync(cancellationToken);

        List<SearchDetailLink> links = [];

        if (todo.SubItems != null)
        {
            links.AddRange(todo.SubItems.Select(s => new SearchDetailLink
            {
                Type = "todoSubItem",
                Id = s.Id,
                Description = s.Description
            }));
        }

        if (todo.Attachments != null)
        {
            links.AddRange(todo.Attachments.Select(a => new SearchDetailLink
            {
                Type = "attachment",
                Id = a.Id,
                Description = a.OriginalFileName
            }));
        }

        var response = new SearchDetailResponse
        {
            Type = "todoItem",
            Id = todo.Id,
            Title = todo.Description,
            Subtitle = todo.Priority.ToString(),
            Summary = new SearchDetailSummary
            {
                CreatedBy = owner is null ? string.Empty : owner.FirstName + " " + owner.LastName,
                CreatedOn = todo.CreatedOn,
                UpdatedOn = todo.UpdatedOn,
                Status = todo.IsCompleted ? "Completed" : "Active"
            },
            Links = links,
            Data = new
            {
                todo.Id,
                todo.Description,
                Priority = (int)todo.Priority,
                todo.DueDate,
                todo.IsCompleted,
                Subtasks = todo.SubItems?.Select(s => new { s.Id, s.Description, s.IsCompleted }).ToList(),
                Attachments = todo.Attachments?.Select(a => new { a.Id, a.OriginalFileName, a.ContentType, a.Size }).ToList()
            }
        };

        return response;
    }

    private async Task<Result<SearchDetailResponse>> HandleSubItem(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        TodoSubItem? subItem = await context.TodoSubItems
            .AsNoTracking()
            .Where(s => s.Id == query.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (subItem is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        TodoItem? parentTask = await context.TodoItems
            .AsNoTracking()
            .Where(t => t.Id == subItem.TodoItemId && t.UserId == query.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (parentTask is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        var response = new SearchDetailResponse
        {
            Type = "todoSubItem",
            Id = subItem.Id,
            Title = subItem.Description,
            Subtitle = parentTask.Description,
            Summary = new SearchDetailSummary
            {
                CreatedBy = string.Empty,
                CreatedOn = subItem.CreatedOn,
                UpdatedOn = subItem.UpdatedOn,
                Status = subItem.IsCompleted ? "Completed" : "Active"
            },
            Links =
            [
                new() { Type = "TodoItem", Id = parentTask.Id, Description = parentTask.Description }
            ],
            Data = new { subItem.Id, subItem.Description, subItem.IsCompleted, TaskId = parentTask.Id }
        };

        return response;
    }

    private async Task<Result<SearchDetailResponse>> HandleAttachment(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        TodoAttachment? attachment = await context.TodoAttachments
            .AsNoTracking()
            .Where(a => a.Id == query.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (attachment is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        TodoItem? todoItem = await context.TodoItems
            .AsNoTracking()
            .Where(t => t.Id == attachment.TodoItemId && t.UserId == query.UserId)
            .SingleOrDefaultAsync(cancellationToken);

        if (todoItem is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        var response = new SearchDetailResponse
        {
            Type = "attachment",
            Id = attachment.Id,
            Title = attachment.OriginalFileName,
            Subtitle = todoItem.Description,
            Summary = new SearchDetailSummary
            {
                CreatedBy = string.Empty,
                CreatedOn = attachment.CreatedOn,
                UpdatedOn = attachment.UpdatedOn,
                Status = string.Empty
            },
            Links =
            [
                new() { Type = "todoItem", Id = todoItem.Id, Description = todoItem.Description }
            ],
            Data = new { attachment.Id, attachment.OriginalFileName, attachment.ContentType, attachment.Size, TaskId = todoItem.Id }
        };

        return response;
    }

    private async Task<Result<SearchDetailResponse>> HandleUser(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        var userEntity = await context.Users
            .AsNoTracking()
            .Where(u => u.Id == query.Id)
            .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.CreatedOn, u.UpdatedOn })
            .SingleOrDefaultAsync(cancellationToken);

        if (userEntity is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        var response = new SearchDetailResponse
        {
            Type = "user",
            Id = userEntity.Id,
            Title = userEntity.FirstName + " " + userEntity.LastName,
            Subtitle = userEntity.Email,
            Summary = new SearchDetailSummary
            {
                CreatedBy = string.Empty,
                CreatedOn = userEntity.CreatedOn,
                UpdatedOn = userEntity.UpdatedOn,
                Status = string.Empty
            },
            Links = [],
            Data = new { userEntity.Id, DisplayName = userEntity.FirstName + " " + userEntity.LastName, userEntity.Email }
        };

        return response;
    }
}
