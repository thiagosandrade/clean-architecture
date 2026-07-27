using Application.Common.Interfaces;
using Application.Elastic.Documents;
using Application.Elastic.Services;
using Domain.API;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Search;

internal sealed class GetSearchDetailQueryHandler(IElasticTodoSearchService elasticTodoSearchService, IElasticUserSearchService elasticUserSearchService)
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
        TodoDocument? todo = await elasticTodoSearchService.GetSearchDetailAsync(query.Id, query.UserId, cancellationToken);

        if (todo is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        UserSearchDocument? owner =  await elasticUserSearchService.GetUserAsync(query.UserId, cancellationToken);

        List<SearchDetailLink> links = [];

        if (todo.Subtasks != null)
        {
            links.AddRange(todo.Subtasks.Select(s => new SearchDetailLink
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
            Subtitle = ((Priority)todo.Priority).ToString(),
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
                todo.Priority,
                todo.DueDate,
                todo.IsCompleted,
                Subtasks = todo.Subtasks?.Select(s => new { s.Id, s.Description, s.IsCompleted }).ToList(),
                Attachments = todo.Attachments?.Select(a => new { a.Id, a.OriginalFileName, a.ContentType, a.Size }).ToList()
            }
        };

        return response;
    }

    private async Task<Result<SearchDetailResponse>> HandleSubItem(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        TodoDocument? parentTask = await elasticTodoSearchService.GetSearchDetailAsync(query.Id, query.UserId, cancellationToken);


        if (parentTask is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        TodoSubtaskDocument? subItem = parentTask.Subtasks.SingleOrDefault(x => x.Id == query.Id);

        if (subItem is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        return new SearchDetailResponse
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
                new()
            {
                Type = "todoItem",
                Id = parentTask.Id,
                Description = parentTask.Description
            }
            ],
            Data = new
            {
                subItem.Id,
                subItem.Description,
                subItem.IsCompleted,
                TaskId = parentTask.Id
            }
        };
    }
    private async Task<Result<SearchDetailResponse>> HandleAttachment(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        TodoDocument? todoItem = await elasticTodoSearchService.GetSearchDetailAsync(query.Id, query.UserId, cancellationToken);


        if (todoItem is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        TodoAttachmentDocument? attachment = todoItem.Attachments.SingleOrDefault(x => x.Id == query.Id);

        if (attachment is null)
        {
            return Result.Failure<SearchDetailResponse>(UserErrors.NotFound(query.Id));
        }

        return new SearchDetailResponse
        {
            Type = "attachment",
            Id = attachment.Id,
            Title = attachment.OriginalFileName,
            Subtitle = todoItem.Description,
            Summary = new SearchDetailSummary
            {
                CreatedBy = string.Empty,
                CreatedOn = todoItem.CreatedOn,
                UpdatedOn = todoItem.UpdatedOn, 
                Status = todoItem.IsCompleted ? "Completed" : "Active"
            },
            Links =
            [
                new()
            {
                Type = "todoItem",
                Id = todoItem.Id,
                Description = todoItem.Description
            }
            ],
            Data = new
            {
                attachment.Id,
                attachment.OriginalFileName,
                attachment.ContentType,
                attachment.Size,
                TaskId = todoItem.Id
            }
        };
    }
    private async Task<Result<SearchDetailResponse>> HandleUser(GetSearchDetailQuery query, CancellationToken cancellationToken)
    {
        UserSearchDocument? userEntity = await elasticUserSearchService.GetUserAsync(query.UserId, cancellationToken);

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
