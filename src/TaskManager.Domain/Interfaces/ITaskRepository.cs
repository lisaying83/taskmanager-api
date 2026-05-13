using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Interfaces;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId, WorkTaskStatus? status = null, CancellationToken ct = default);
    Task AddAsync(TaskItem task, CancellationToken ct = default);
    void Update(TaskItem task);
}
