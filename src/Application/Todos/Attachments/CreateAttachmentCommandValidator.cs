using FluentValidation;

namespace Application.Todos.Attachments;

public sealed class CreateAttachmentCommandValidator : AbstractValidator<CreateAttachmentCommand>
{
    public CreateAttachmentCommandValidator()
    {
        RuleFor(x => x.TodoId).NotEmpty();
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OriginalFileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.StoredFileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Size).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Data).NotNull().NotEmpty();
    }
}
