using Application.Abstractions.Messaging;

namespace Application.Todos.Attachments;

public sealed record GetAttachmentFileQuery(Guid TodoId, Guid AttachmentId, Guid UserId) : IQuery<AttachmentFileResponse>;

public sealed class AttachmentFileResponse
{
    public AttachmentFileResponse(byte[] data, string contentType, string originalFileName)
    {
        Data = data;
        ContentType = contentType;
        OriginalFileName = originalFileName;
    }

    public byte[] Data { get; }
    public string ContentType { get; }
    public string OriginalFileName { get; }
}
