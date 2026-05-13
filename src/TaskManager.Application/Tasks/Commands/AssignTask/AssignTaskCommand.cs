using MediatR;

namespace TaskManager.Application.Tasks.Commands.AssignTask;

public sealed record AssignTaskCommand(Guid TaskId, Guid AssignedToUserId, Guid AssignedByUserId) : IRequest;
