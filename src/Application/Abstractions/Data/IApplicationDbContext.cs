using Domain.Permissions;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<UserPermission> UserPermissions { get; }
    DbSet<TodoItem> TodoItems { get; }
    DbSet<TodoSubItem> TodoSubItems { get; }
    DbSet<Permission> Permissions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
