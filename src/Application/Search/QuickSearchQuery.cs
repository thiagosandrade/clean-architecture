using Application.Abstractions.Messaging;
using SharedKernel;
using System;
using System.Collections.Generic;

namespace Application.Search;

public sealed record QuickSearchQuery(Guid UserId, string Text, int Limit = 5) : IQuery<QuickSearchResponse>;

public sealed class QuickSearchResponse
{
    public List<TaskSearchItem> Tasks { get; init; } = new();
    public List<SubtaskSearchItem> Subtasks { get; init; } = new();
    public List<AttachmentSearchItem> Attachments { get; init; } = new();
    public List<UserSearchItem> Users { get; init; } = new();
}

public sealed class TaskSearchItem
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public int Priority { get; init; }
    public DateTime? DueDate { get; init; }
    public bool Completed { get; init; }
}

public sealed class SubtaskSearchItem
{
    public Guid Id { get; init; }
    public string Description { get; init; }
    public bool Completed { get; init; }
    public Guid TaskId { get; init; }
    public string TaskDescription { get; init; }
}

public sealed class AttachmentSearchItem
{
    public Guid Id { get; init; }
    public string OriginalFileName { get; init; }
    public string ContentType { get; init; }
    public long Size { get; init; }
    public Guid TaskId { get; init; }
    public string TaskDescription { get; init; }
}

public sealed class UserSearchItem
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; }
    public string Email { get; init; }
}
