using SharedKernel.Common;

namespace Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid userId) => Error.NotFound(
        "Users.NotFound",
        $"The user with the Id = '{userId}' was not found");

    public static Error PermissionNotFound(Guid permissionId) => Error.NotFound(
        "Permission.NotFound",
        $"The permission requested '{permissionId}' was not available");

    public static Error PermissionNotExistsForUser(Guid userId, Guid permissionId) => Error.Conflict(
        "Permission.NotExistsForUser",
        $"The permission requested '{permissionId}' not exists for user '{userId}'");

    public static Error PermissionAlreadyExistsForUser(Guid userId, Guid permissionId) => Error.Conflict(
        "Permission.AlreadyExistsForUser",
        $"The permission requested '{permissionId}' already exists for user '{userId}'");

    public static Error Unauthorized() => Error.Failure(
        "Users.Unauthorized",
        "You are not authorized to perform this action.");

    public static readonly Error NotFoundByEmail = Error.NotFound(
        "Users.NotFoundByEmail",
        "The user with the specified email was not found");

    public static readonly Error EmailNotUnique = Error.Conflict(
        "Users.EmailNotUnique",
        "The provided email is not unique");
}
