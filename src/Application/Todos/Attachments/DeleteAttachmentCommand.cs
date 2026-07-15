using Application.Abstractions.Messaging;

namespace Application.Todos.Attachments;

public sealed record DeleteAttachmentCommand(Guid TodoId, Guid AttachmentId, Guid UserId) : ICommand;
