using Application.Abstractions.Messaging;
using Application.Users.Login;

namespace Application.Users.GetById;

public sealed record GetUserByIdQuery(Guid UserId) : IQuery<UserResponse>;
