using Domain.Todos;

namespace Application.Todos.Attachments;

public sealed class AttachmentResponse
{
    public AttachmentResponse(TaskAttachment attachment)
    {
        Id = attachment.Id;
        TodoItemId = attachment.TodoItemId;
        OriginalFileName = attachment.OriginalFileName;
        StoredFileName = attachment.StoredFileName;
        ContentType = attachment.ContentType;
        Size = attachment.Size;
        CreatedOn = attachment.CreatedOn;
        CreatedBy = attachment.CreatedBy;
    }

    public Guid Id { get; }
    public Guid TodoItemId { get; }
    public string OriginalFileName { get; }
    public string StoredFileName { get; }
    public string ContentType { get; }
    public long Size { get; }
    public DateTime CreatedOn { get; }
    public Guid CreatedBy { get; }
}
