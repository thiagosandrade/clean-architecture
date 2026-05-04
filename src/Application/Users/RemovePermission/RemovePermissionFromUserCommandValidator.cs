using FluentValidation;

namespace Application.Users.RemovePermission;

internal sealed class RemovePermissionFromUserCommandValidator : AbstractValidator<RemovePermissionFromUserCommand>
{
    public RemovePermissionFromUserCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.PermissionId).NotEmpty();
    }
}
