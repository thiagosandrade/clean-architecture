using FluentValidation;

namespace Application.Users.SetPermission;

internal sealed class SetPermissionToUserCommandValidator : AbstractValidator<SetPermissionToUserCommand>
{
    public SetPermissionToUserCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.PermissionId).NotEmpty();
    }
}
