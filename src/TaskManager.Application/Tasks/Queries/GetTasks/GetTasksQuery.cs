using MediatR;
using TaskManager.Domain.Enums;
using TaskManager.Application.Tasks.Queries.GetTask;

namespace TaskManager.Application.Tasks.Queries.GetTasks;

public sealed record GetTasksQuery(Guid UserId, WorkTaskStatus? Status = null) : IRequest<IReadOnlyList<TaskDto>>;
