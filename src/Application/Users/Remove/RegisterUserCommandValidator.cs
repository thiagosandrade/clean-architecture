using FluentValidation;

namespace Application.Users.Remove;

internal sealed class RemoveUserCommandValidator : AbstractValidator<RemoveUserCommand>
{
    public RemoveUserCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}
