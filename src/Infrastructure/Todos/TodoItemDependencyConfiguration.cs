using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Todos;

internal sealed class TodoItemDependencyConfiguration : IEntityTypeConfiguration<TodoDependency>
{
    public void Configure(EntityTypeBuilder<TodoDependency> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();

        builder.HasOne(d => d.TodoItem)
            .WithMany(t => t.Dependencies)
            .HasForeignKey(d => d.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.DependsOnTodoItem)
            .WithMany()
            .HasForeignKey(d => d.DependsOnTodoItemId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
