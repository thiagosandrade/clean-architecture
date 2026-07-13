using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Todos;

internal sealed class TaskItemDependencyConfiguration : IEntityTypeConfiguration<TaskDependency>
{
    public void Configure(EntityTypeBuilder<TaskDependency> builder)
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
