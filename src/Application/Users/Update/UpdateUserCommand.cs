using SharedKernel.Abstractions.Messaging;

namespace Application.Users.Update;

public sealed record UpdateUserCommand(Guid Id, string Email, string FirstName, string LastName, string Password)
    : ICommand<Guid>;
