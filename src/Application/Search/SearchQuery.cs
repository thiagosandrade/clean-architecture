using Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Search;

public sealed record SearchQuery(Guid UserId, string Text, int Page = 1, int PageSize = 20) : IQuery<SearchResponse>;

public sealed class SearchResponse
{
    public List<TodoItemSearch> Tasks { get; init; } = [];
    public List<TodoSubItemSearch> Subtasks { get; init; } = [];
    public List<TodoItemAttachmentSearch> Attachments { get; init; } = [];
    public List<UserItemSearch> Users { get; init; } = [];
}
