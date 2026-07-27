using Application.Common.Interfaces;
using Application.Elastic.Documents;
using Application.Elastic.Services;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Search;

internal sealed class GetQuickSearchQueryHandler(
    IElasticTodoSearchService elasticTodoSearchService,
    IElasticUserSearchService elasticUsersSearchService)
    : IQueryHandler<GetQuickSearchQuery, QuickSearchResponse>
{
    public async Task<Result<QuickSearchResponse>> Handle(GetQuickSearchQuery query, CancellationToken cancellationToken)
    {
        int limit = query.Limit <= 0 ? 5 : query.Limit;

        Task<List<TodoDocument>> todoDocumentsTask = elasticTodoSearchService.GetQuickSearchDetailAsync(query.UserId, query.Text, limit, cancellationToken);

        Task<List<UserSearchDocument>> userDocumentsTask = elasticUsersSearchService.SearchUsersAsync(query.Text, limit, cancellationToken);

        await Task.WhenAll(todoDocumentsTask, userDocumentsTask);

        List<TodoDocument> todoDocuments = await todoDocumentsTask;
        List<UserSearchDocument> userDocuments = await userDocumentsTask;

        List<TodoItemSearch> todoItems =
            [
                .. todoDocuments
                    .Where(t =>
                        t.Description.Contains(
                            query.Text,
                            StringComparison.OrdinalIgnoreCase))
                    .Select(t => new TodoItemSearch
                    {
                        Id = t.Id,
                        Description = t.Description,
                        Priority = t.Priority,
                        DueDate = t.DueDate,
                        Completed = t.IsCompleted
                    })
            ];

        List<TodoSubItemSearch> subtasks =
            [
                .. todoDocuments
                    .SelectMany(todo =>
                        todo.Subtasks
                            .Where(s =>
                                s.Description.Contains(
                                    query.Text,
                                    StringComparison.OrdinalIgnoreCase))
                            .Select(s => new TodoSubItemSearch
                            {
                                Id = s.Id,
                                Description = s.Description,
                                Completed = s.IsCompleted,
                                TaskId = todo.Id,
                                TaskDescription = todo.Description
                            }))
            ];

        List<TodoItemAttachmentSearch> attachments =
            [
                .. todoDocuments
                    .SelectMany(todo =>
                        todo.Attachments
                            .Where(a =>
                                a.OriginalFileName.Contains(
                                    query.Text,
                                    StringComparison.OrdinalIgnoreCase))
                            .Select(a => new TodoItemAttachmentSearch
                            {
                                Id = a.Id,
                                OriginalFileName = a.OriginalFileName,
                                ContentType = a.ContentType,
                                Size = a.Size,
                                TaskId = todo.Id,
                                TaskDescription = todo.Description
                            }))
            ];

        List<UserItemSearch> users =
            [
                .. userDocuments.Select(u => new UserItemSearch
                {
                    Id = u.Id,
                    DisplayName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email
                })
            ];

        return new QuickSearchResponse
        {
            Tasks = todoItems,
            Subtasks = subtasks,
            Attachments = attachments,
            Users = users
        };
    }
}
