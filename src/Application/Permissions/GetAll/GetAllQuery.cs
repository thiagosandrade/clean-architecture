using Application.Abstractions.Messaging;

namespace Application.Permissions.GetAll;

public sealed record GetAllQuery() : IQuery<List<PermissionResponse>>;
