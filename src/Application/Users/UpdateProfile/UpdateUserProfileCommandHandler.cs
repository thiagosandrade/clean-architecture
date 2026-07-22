using Application.Common.Interfaces;
using Application.RabbitMq.Configuration;
using Application.RabbitMq.Events;
using Domain.API;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Abstractions.Messaging;

namespace Application.Users.UpdateProfile;

internal sealed class UpdateUserProfileCommandHandler(
    IApplicationDbContext context,
    IRabbitMqPublisher publisher,
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

        await publisher.PublishAsync(new UserUpdatedIntegrationEvent(user.Id), cancellationToken);

        return user.Id;
    }
}
