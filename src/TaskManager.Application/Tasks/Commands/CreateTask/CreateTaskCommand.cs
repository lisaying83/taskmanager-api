using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    TaskPriority Priority,
    Guid CreatedByUserId,
    DateTime? DueDate) : IRequest<CreateTaskResult>;

public sealed record CreateTaskResult(Guid TaskId, string Title, WorkTaskStatus Status);
