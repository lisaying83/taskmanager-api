using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class TaskRepository(AppDbContext db) : ITaskRepository
{
    public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<TaskItem>> GetByUserAsync(
        Guid userId, WorkTaskStatus? status = null, CancellationToken ct = default)
    {
        var query = db.Tasks.Where(t => t.CreatedByUserId == userId || t.AssignedToUserId == userId);

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync(ct);
    }

    public async Task AddAsync(TaskItem task, CancellationToken ct = default) =>
        await db.Tasks.AddAsync(task, ct);

    public void Update(TaskItem task) => db.Tasks.Update(task);
}
