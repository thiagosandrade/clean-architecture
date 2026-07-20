using Domain.Activities;
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
    DbSet<TodoActivity> TodoActivities { get; }
    DbSet<TodoDependency> TodoDependencies { get; }
    DbSet<TodoAttachment> TodoAttachments { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
