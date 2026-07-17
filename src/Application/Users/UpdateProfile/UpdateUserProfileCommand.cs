using Application.Abstractions.Messaging;

namespace Application.Users.UpdateProfile;

public sealed record UpdateUserProfileCommand(Guid Id, string Email, string FirstName, string LastName)
    : ICommand<Guid>;
