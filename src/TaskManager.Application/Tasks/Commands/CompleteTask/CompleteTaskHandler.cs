using MediatR;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tasks.Commands.CompleteTask;

public sealed class CompleteTaskHandler(
    ITaskRepository taskRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteTaskCommand>
{
    public async Task Handle(CompleteTaskCommand request, CancellationToken ct)
    {
        var task = await taskRepository.GetByIdAsync(request.TaskId, ct)
            ?? throw new DomainException("Task not found.");

        task.Complete(request.UserId);

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
