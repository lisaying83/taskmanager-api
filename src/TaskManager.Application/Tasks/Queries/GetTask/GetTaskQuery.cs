using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Tasks.Queries.GetTask;

public sealed record GetTaskQuery(Guid TaskId) : IRequest<TaskDto?>;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string Description,
    WorkTaskStatus Status,
    TaskPriority Priority,
    Guid CreatedByUserId,
    Guid? AssignedToUserId,
    DateTime CreatedAt,
    DateTime? DueDate,
    DateTime? CompletedAt);
