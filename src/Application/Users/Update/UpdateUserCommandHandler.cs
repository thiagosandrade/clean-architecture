using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.Update;

internal sealed class UpdateUserCommandHandler(
    IApplicationDbContext context,
    IRabbitMqPublisher publisher,
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

        await publisher.PublishAsync(new UserUpdatedIntegrationEvent(user.Id), cancellationToken);

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
