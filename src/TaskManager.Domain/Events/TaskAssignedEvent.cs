namespace TaskManager.Domain.Events;

public sealed record TaskAssignedEvent(
    Guid Id,
    Guid TaskId,
    Guid AssignedToUserId,
    Guid AssignedByUserId,
    DateTime OccurredOn) : IDomainEvent;
