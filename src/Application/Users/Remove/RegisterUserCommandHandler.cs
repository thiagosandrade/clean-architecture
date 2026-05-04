using Application.Abstractions.Authentication;
using Application.Abstractions.Constants;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Users.Remove;

internal sealed class RemoveUserCommandHandler(IApplicationDbContext context)
    : ICommandHandler<RemoveUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(RemoveUserCommand command, CancellationToken cancellationToken)
    {
        if (!await context.Users.AnyAsync(u => u.Id == command.UserId, cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.NotFound(command.UserId));
        }

        User user = await context.Users.FirstAsync(x => x.Id == command.UserId, cancellationToken);

        user.Raise(new UserRemovedDomainEvent(user.Id));

        context.Users.Remove(user);

        await context.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
