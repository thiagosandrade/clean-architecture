using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Abstractions.Constants;

namespace Infrastructure.Persistence.Users;

internal sealed class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasData(
            new UserPermission
            {
                Id = new Guid("019f6b1a-f9a3-7ff2-bb54-9223cdf5d34f"),
                UserId = UserConstants.JamesBondId,
                PermissionId = PermissionsConstants.PermissionAccessId,
                CreatedOn = new DateTime(2026, 7, 16, 13, 25, 57, DateTimeKind.Utc)
            },
            new UserPermission
            {
                Id = new Guid("019f6b1c-a541-7310-a652-021d00ca5fe8"),
                UserId = UserConstants.JamesBondId,
                PermissionId = PermissionsConstants.TodoAccessId,
                CreatedOn = new DateTime(2026, 7, 16, 13, 27, 47, DateTimeKind.Utc)
            },
            new UserPermission
            {
                Id = new Guid("019f6b1d-6e35-71e4-a136-37cd0f95ad0d"),
                UserId = UserConstants.JamesBondId,
                PermissionId = PermissionsConstants.ActivityAccessId,
                CreatedOn = new DateTime(2026, 7, 16, 13, 28, 38, DateTimeKind.Utc)
            },
            new UserPermission
            {
                Id = new Guid("019f6b1d-db36-7d5a-ba4b-9be1aca1137b"),
                UserId = UserConstants.JamesBondId,
                PermissionId = PermissionsConstants.UsersAccessId,
                CreatedOn = new DateTime(2026, 7, 16, 13, 29, 06, DateTimeKind.Utc)
            });
    }
}
