using MediatR;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tasks.Commands.AssignTask;

public sealed class AssignTaskHandler(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignTaskCommand>
{
    public async Task Handle(AssignTaskCommand request, CancellationToken ct)
    {
        var task = await taskRepository.GetByIdAsync(request.TaskId, ct)
            ?? throw new DomainException("Task not found.");

        var targetUser = await userRepository.GetByIdAsync(request.AssignedToUserId, ct);
        if (targetUser is null)
            throw new DomainException("Target user not found.");

        task.Assign(request.AssignedToUserId, request.AssignedByUserId);

        taskRepository.Update(task);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
