using Application.Abstractions.Messaging;

namespace Application.Todos.Attachments;

public sealed record GetAttachmentByIdQuery(Guid TodoId, Guid AttachmentId, Guid UserId) : IQuery<AttachmentResponse>;
