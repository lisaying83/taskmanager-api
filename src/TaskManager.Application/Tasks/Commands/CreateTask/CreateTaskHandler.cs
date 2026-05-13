using MediatR;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskHandler(
    ITaskRepository taskRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTaskCommand, CreateTaskResult>
{
    public async Task<CreateTaskResult> Handle(CreateTaskCommand request, CancellationToken ct)
    {
        var userExists = await userRepository.GetByIdAsync(request.CreatedByUserId, ct);
        if (userExists is null)
            throw new DomainException("User not found.");

        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.CreatedByUserId,
            request.DueDate);

        await taskRepository.AddAsync(task, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return new CreateTaskResult(task.Id, task.Title.Value, task.Status);
    }
}
