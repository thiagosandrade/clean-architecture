using SharedKernel.Abstractions.Messaging;

namespace Application.Todos.Attachments;

public sealed record GetAttachmentsQuery(Guid TodoId, Guid UserId) : IQuery<AttachmentsResponse>;

public sealed class AttachmentsResponse
{
    public AttachmentsResponse(IEnumerable<AttachmentResponse> attachments)
    {
        Attachments = [.. attachments];
    }

    public IEnumerable<AttachmentResponse> Attachments { get; }
}
