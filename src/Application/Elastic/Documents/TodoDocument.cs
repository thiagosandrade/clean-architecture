using Domain.Activities;
using Domain.Todos;

namespace Application.Elastic.Documents;

public sealed class TodoDocument
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Description { get; init; } = string.Empty;
    public int Priority { get; init; }
    public string PriorityAsText { get; init; } = "";
    public bool IsCompleted { get; init; }
    public DateTime? DueDate { get; init; }
    public DateTime CreatedOn { get; init; }
    public DateTime? UpdatedOn { get; init; }
    public DateTime? CompletedOn { get; init; }
    public float[] Embedding { get; init; } = [];
    public List<string> Labels { get; init; } = [];
    public List<string> Categories { get; init; } = [];
    public List<TodoSubtaskDocument> Subtasks { get; init; } = [];
    public List<TodoAttachmentDocument> Attachments { get; init; } = [];
    public List<TodoActivityDocument> Activities { get; init; } = []; 
}

public sealed class TodoSubtaskDocument
{
    public Guid Id { get; init; }
    public Guid SubItemId { get; init; }
    public string Description { get; init; } = string.Empty;
    public bool IsCompleted { get; init; }
    public int Order { get; init; }
    public DateTime? CompletedOn { get; init; }
    public DateTime CreatedOn { get; internal set; }
    public DateTime? UpdatedOn { get; internal set; }
}

public sealed class TodoAttachmentDocument
{
    public Guid Id { get; init; }
    public string OriginalFileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long Size { get; init; }
}

public sealed class TodoActivityDocument
{
    public Guid Id { get; set; }
    public int ActivityType { get; set; }
    public string ActivityTypeAsText { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Metadata { get; set; }
}
