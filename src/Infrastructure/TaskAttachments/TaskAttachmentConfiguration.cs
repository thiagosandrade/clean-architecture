using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.TaskAttachments;

public sealed class TaskAttachmentConfiguration : IEntityTypeConfiguration<TaskAttachment>
{
    public void Configure(EntityTypeBuilder<TaskAttachment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Size)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Property(x => x.Data)
            .IsRequired()
            .HasColumnType("bytea");

        builder.HasIndex(x => x.TodoItemId);
        builder.HasIndex(x => x.CreatedOn);
    }
}
