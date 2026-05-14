using Application.Abstractions.Authentication;
using Application.Abstractions.Constants;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Register;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Update;

internal sealed class UpdateUserCommandHandler(
    IApplicationDbContext context, 
    IPasswordHasher passwordHasher,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (!await CheckIfUserExists(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.Id));
        }

        if (!await CheckIfEmailUnique(command, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        User user = await LoadUser(command, cancellationToken);

        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.PasswordHash = passwordHasher.Hash(command.Password);
        user.UpdatedOn = dateTimeProvider.UtcNow;

        user.Raise(new UserUpdatedDomainEvent(user.Id));

        context.Users.Update(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    private async Task<User> LoadUser(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return await context.Users.FirstAsync(x => x.Id == command.Id, cancellationToken);
    }

    private async Task<bool> CheckIfEmailUnique(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken);
    }

    private async Task<bool> CheckIfUserExists(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        return await context.Users.AnyAsync(u => u.Id == command.Id, cancellationToken);
    }
}
