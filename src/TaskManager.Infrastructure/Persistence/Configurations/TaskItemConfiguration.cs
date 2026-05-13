using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.Entities;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("id");
        builder.Property(t => t.Status).HasColumnName("status");
        builder.Property(t => t.Priority).HasColumnName("priority");
        builder.Property(t => t.CreatedByUserId).HasColumnName("created_by_user_id");
        builder.Property(t => t.AssignedToUserId).HasColumnName("assigned_to_user_id");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");
        builder.Property(t => t.DueDate).HasColumnName("due_date");
        builder.Property(t => t.CompletedAt).HasColumnName("completed_at");

        builder.OwnsOne(t => t.Title, title =>
        {
            title.Property(v => v.Value)
                .HasColumnName("title")
                .HasMaxLength(TaskTitle.MaxLength)
                .IsRequired();
        });

        builder.OwnsOne(t => t.Description, desc =>
        {
            desc.Property(v => v.Value)
                .HasColumnName("description")
                .HasMaxLength(TaskDescription.MaxLength);
        });

        builder.HasIndex(t => t.CreatedByUserId).HasDatabaseName("ix_tasks_created_by_user_id");
        builder.HasIndex(t => t.AssignedToUserId).HasDatabaseName("ix_tasks_assigned_to_user_id");
        builder.HasIndex(t => t.Status).HasDatabaseName("ix_tasks_status");
    }
}
