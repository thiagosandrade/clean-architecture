using Application.Abstractions.Constants;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasData(
            new User
            {
                Id = UserConstants.JamesBondId,
                Email = "james.bond@mail.com",
                FirstName = "james",
                LastName = "bond",
                PasswordHash = "646584747216FCD2E04A09CB831A495213422447104F0DAE7103ACAAB05AD988-3DF7AD47EDD6E4B714EA6898AF9A6B68",
                CreatedOn = new DateTime(2026, 7, 16, 13, 25, 57, DateTimeKind.Utc)
            });
    }
}
