using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Domain.Entities;

public class TaskItem : Entity
{
    public TaskTitle Title { get; private set; } = null!;
    public TaskDescription Description { get; private set; } = null!;
    public WorkTaskStatus Status { get; private set; }
    public TaskPriority Priority { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DueDate { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(
        string title,
        string? description,
        TaskPriority priority,
        Guid createdByUserId,
        DateTime? dueDate = null)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = TaskTitle.Create(title),
            Description = TaskDescription.Create(description),
            Status = WorkTaskStatus.Todo,
            Priority = priority,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate
        };

        task.AddDomainEvent(new TaskCreatedEvent(
            Guid.NewGuid(), task.Id, task.Title.Value, createdByUserId, task.CreatedAt));

        return task;
    }

    public void Assign(Guid assignedToUserId, Guid assignedByUserId)
    {
        if (Status == WorkTaskStatus.Done || Status == WorkTaskStatus.Cancelled)
            throw new DomainException("Cannot assign a completed or cancelled task.");

        AssignedToUserId = assignedToUserId;

        AddDomainEvent(new TaskAssignedEvent(
            Guid.NewGuid(), Id, assignedToUserId, assignedByUserId, DateTime.UtcNow));
    }

    public void Complete(Guid completedByUserId)
    {
        if (Status == WorkTaskStatus.Done)
            throw new DomainException("Task is already completed.");

        if (Status == WorkTaskStatus.Cancelled)
            throw new DomainException("Cannot complete a cancelled task.");

        Status = WorkTaskStatus.Done;
        CompletedAt = DateTime.UtcNow;

        AddDomainEvent(new TaskCompletedEvent(
            Guid.NewGuid(), Id, completedByUserId, CompletedAt.Value));
    }

    public void Cancel()
    {
        if (Status == WorkTaskStatus.Done)
            throw new DomainException("Cannot cancel a completed task.");

        Status = WorkTaskStatus.Cancelled;
    }

    public void UpdateTitle(string title) => Title = TaskTitle.Create(title);

    public void UpdateDescription(string? description) => Description = TaskDescription.Create(description);

    public void UpdatePriority(TaskPriority priority) => Priority = priority;
}
