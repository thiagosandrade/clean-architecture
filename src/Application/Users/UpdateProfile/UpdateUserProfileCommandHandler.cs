using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IApplicationDbContext context,
    IDateTimeProvider dateTimeProvider)
    : ICommandHandler<UpdateUserProfileCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        bool exists = await context.Users.AnyAsync(u => u.Id == command.Id, cancellationToken);
        if (!exists)
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.Id));
        }

        bool emailTaken = await context.Users.AnyAsync(u => u.Email == command.Email && u.Id != command.Id, cancellationToken);
        if (emailTaken)
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        User user = await context.Users.FirstAsync(u => u.Id == command.Id, cancellationToken);

        user.Email = command.Email;
        user.FirstName = command.FirstName;
        user.LastName = command.LastName;
        user.UpdatedOn = dateTimeProvider.UtcNow;

        user.Raise(new UserUpdatedDomainEvent(user.Id));

        context.Users.Update(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
