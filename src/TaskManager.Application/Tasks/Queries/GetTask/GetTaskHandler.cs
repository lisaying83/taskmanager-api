using MediatR;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tasks.Queries.GetTask;

public sealed class GetTaskHandler(ITaskRepository taskRepository) : IRequestHandler<GetTaskQuery, TaskDto?>
{
    public async Task<TaskDto?> Handle(GetTaskQuery request, CancellationToken ct)
    {
        var task = await taskRepository.GetByIdAsync(request.TaskId, ct);
        if (task is null) return null;

        return new TaskDto(
            task.Id,
            task.Title.Value,
            task.Description.Value,
            task.Status,
            task.Priority,
            task.CreatedByUserId,
            task.AssignedToUserId,
            task.CreatedAt,
            task.DueDate,
            task.CompletedAt);
    }
}
