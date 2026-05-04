using Application.Abstractions.Authentication;
using Application.Abstractions.Constants;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Users.Register;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Update;

internal sealed class UpdateUserCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    : ICommandHandler<UpdateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Id == command.Id, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.Id));
        }

        if (await context.Users.AnyAsync(u => u.Email == command.Email, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        User user = await context.Users.FirstAsync(x => x.Id ==  command.Id, cancellationToken);

        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.PasswordHash = passwordHasher.Hash(command.Password);

        user.Raise(new UserUpdatedDomainEvent(user.Id));

        context.Users.Update(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
