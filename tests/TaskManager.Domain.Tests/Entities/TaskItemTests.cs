using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Events;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Tests.Entities;

public class TaskItemTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidData_ReturnsTaskWithTodoStatus()
    {
        var task = TaskItem.Create("Fix login bug", "Details here", TaskPriority.High, UserId);

        task.Title.Value.Should().Be("Fix login bug");
        task.Status.Should().Be(WorkTaskStatus.Todo);
        task.Priority.Should().Be(TaskPriority.High);
        task.CreatedByUserId.Should().Be(UserId);
    }

    [Fact]
    public void Create_RaisesTaskCreatedEvent()
    {
        var task = TaskItem.Create("Fix login bug", null, TaskPriority.Medium, UserId);

        task.DomainEvents.Should().ContainSingle(e => e is TaskCreatedEvent);
    }

    [Fact]
    public void Create_WithEmptyTitle_ThrowsDomainException()
    {
        var act = () => TaskItem.Create("  ", null, TaskPriority.Low, UserId);

        act.Should().Throw<DomainException>().WithMessage("*title*");
    }

    [Fact]
    public void Complete_TodoTask_SetsStatusToDone()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.ClearDomainEvents();

        task.Complete(UserId);

        task.Status.Should().Be(WorkTaskStatus.Done);
        task.CompletedAt.Should().NotBeNull();
        task.DomainEvents.Should().ContainSingle(e => e is TaskCompletedEvent);
    }

    [Fact]
    public void Complete_AlreadyDoneTask_ThrowsDomainException()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.Complete(UserId);

        var act = () => task.Complete(UserId);

        act.Should().Throw<DomainException>().WithMessage("*already completed*");
    }

    [Fact]
    public void Complete_CancelledTask_ThrowsDomainException()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.Cancel();

        var act = () => task.Complete(UserId);

        act.Should().Throw<DomainException>().WithMessage("*cancelled*");
    }

    [Fact]
    public void Cancel_DoneTask_ThrowsDomainException()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.Complete(UserId);

        var act = () => task.Cancel();

        act.Should().Throw<DomainException>().WithMessage("*completed*");
    }

    [Fact]
    public void Assign_ActiveTask_RaisesTaskAssignedEvent()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.ClearDomainEvents();
        var assignee = Guid.NewGuid();

        task.Assign(assignee, UserId);

        task.AssignedToUserId.Should().Be(assignee);
        task.DomainEvents.Should().ContainSingle(e => e is TaskAssignedEvent);
    }

    [Fact]
    public void Assign_CompletedTask_ThrowsDomainException()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.Complete(UserId);

        var act = () => task.Assign(Guid.NewGuid(), UserId);

        act.Should().Throw<DomainException>().WithMessage("*assign*completed*");
    }

    [Fact]
    public void ClearDomainEvents_RemovesAllEvents()
    {
        var task = TaskItem.Create("Task", null, TaskPriority.Low, UserId);
        task.DomainEvents.Should().NotBeEmpty();

        task.ClearDomainEvents();

        task.DomainEvents.Should().BeEmpty();
    }
}
