using MediatR;
using TaskManager.Application.Tasks.Queries.GetTask;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tasks.Queries.GetTasks;

public sealed class GetTasksHandler(ITaskRepository taskRepository) : IRequestHandler<GetTasksQuery, IReadOnlyList<TaskDto>>
{
    public async Task<IReadOnlyList<TaskDto>> Handle(GetTasksQuery request, CancellationToken ct)
    {
        var tasks = await taskRepository.GetByUserAsync(request.UserId, request.Status, ct);

        return tasks.Select(t => new TaskDto(
            t.Id,
            t.Title.Value,
            t.Description.Value,
            t.Status,
            t.Priority,
            t.CreatedByUserId,
            t.AssignedToUserId,
            t.CreatedAt,
            t.DueDate,
            t.CompletedAt)).ToList();
    }
}
