using SharedKernel;

namespace Domain.Todos;

public sealed class TaskAttachment : Entity
{
    public Guid Id { get; set; }

    public Guid TodoItemId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string StoredFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long Size { get; set; }

    public Guid CreatedBy { get; set; }

    // Binary content of the uploaded file
    public byte[] Data { get; set; } = [];
}
