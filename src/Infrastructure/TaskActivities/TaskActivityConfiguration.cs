using System;
using System.Collections.Generic;
using System.Text;
using Domain.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.TaskActivities;

public sealed class TodoActivityConfiguration
    : IEntityTypeConfiguration<TaskActivity>
{
    public void Configure(EntityTypeBuilder<TaskActivity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(x => x.TodoItem)
            .WithMany(x => x.TaskActivities)
            .HasForeignKey(x => x.TodoItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TodoItemId);

        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
