using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Todos;

internal sealed class TodoItemAttachmentConfiguration : IEntityTypeConfiguration<TodoAttachment>
{
    public void Configure(EntityTypeBuilder<TodoAttachment> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(x => x.CreatedOn)
            .HasDefaultValueSql("NOW()")
            .ValueGeneratedOnAdd();
        
        builder.HasOne(x => x.TodoItem)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
