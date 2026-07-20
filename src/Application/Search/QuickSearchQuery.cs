using Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Search;

public sealed record QuickSearchQuery(Guid UserId, string Text, int Limit = 5) : IQuery<QuickSearchResponse>;

public sealed class QuickSearchResponse
{
    public List<TodoItemSearch> Tasks { get; init; } = [];
    public List<TodoSubItemSearch> Subtasks { get; init; } = [];
    public List<TodoItemAttachmentSearch> Attachments { get; init; } = [];
    public List<UserItemSearch> Users { get; init; } = [];
}

public sealed class TodoItemSearch
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public int Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public bool Completed { get; init; }
}

public sealed class TodoSubItemSearch
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public bool Completed { get; init; }
    public Guid TaskId { get; init; }
    public string TaskDescription { get; init; }
}

public sealed class TodoItemAttachmentSearch
{
    public Guid Id { get; init; }
    public string OriginalFileName { get; init; }
    public string ContentType { get; init; }
    public long Size { get; init; }
    public Guid TaskId { get; init; }
    public string TaskDescription { get; init; }
}

public sealed class UserItemSearch
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; }
    public string Email { get; init; }
}
