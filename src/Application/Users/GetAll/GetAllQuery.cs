using SharedKernel.Abstractions.Messaging;

namespace Application.Users.GetAll;

public sealed record GetAllQuery() : IQuery<List<UserResponse>>;
