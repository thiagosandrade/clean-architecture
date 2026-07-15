using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Todos.Attachments;

public sealed record CreateAttachmentCommand(
    Guid TodoId,
    Guid UserId,
    string OriginalFileName,
    string StoredFileName,
    string ContentType,
    long Size,
    byte[] Data) : ICommand<Guid>;
