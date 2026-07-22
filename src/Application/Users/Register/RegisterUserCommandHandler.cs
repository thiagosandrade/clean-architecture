using Application.Common.Interfaces;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Constants;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.Register;

internal sealed class RegisterUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    : ICommandHandler<RegisterUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = passwordHasher.Hash(command.Password)
        };

        user.Raise(new UserDomainEvents(user.Id));

        context.Users.Add(user);

        context.UserPermissions.Add(new UserPermission()
        {
            UserId = user.Id,
            PermissionId = PermissionsConstants.UsersAccessId
        });

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
