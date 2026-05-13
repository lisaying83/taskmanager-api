using MediatR;

namespace TaskManager.Application.Tasks.Commands.CompleteTask;

public sealed record CompleteTaskCommand(Guid TaskId, Guid UserId) : IRequest;
