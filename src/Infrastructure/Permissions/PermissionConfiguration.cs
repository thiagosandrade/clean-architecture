using Application.Abstractions.Constants;
using Domain.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Permissions;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Description).IsUnique();

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasData(
            new Permission
            {
                Id = PermissionsConstants.TodoAccessId,
                Description = PermissionsConstants.TodoAccess
            },
            new Permission
            {
                Id = PermissionsConstants.UsersAccessId,
                Description = PermissionsConstants.UsersAccess
            },
            new Permission
            {
                Id = PermissionsConstants.PermissionAccessId,
                Description = PermissionsConstants.PermissionAccess
            },
            new Permission
            {
                Id = PermissionsConstants.ActivityAccessId,
                Description = PermissionsConstants.ActivityAccess
            }
        );
    }
}
