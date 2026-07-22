using System.Reflection.Emit;
using System.Reflection.Metadata;
using Domain.Todos;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Todos;

internal sealed class TodoSubItemConfiguration : IEntityTypeConfiguration<TodoSubItem>
{
    public void Configure(EntityTypeBuilder<TodoSubItem> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasOne(x => x.TodoItem)
            .WithMany(x => x.SubItems)
            .HasForeignKey(x => x.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();
    }
}
