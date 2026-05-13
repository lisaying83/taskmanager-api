namespace TaskManager.Domain.Events;

public sealed record TaskCompletedEvent(
    Guid Id,
    Guid TaskId,
    Guid CompletedByUserId,
    DateTime OccurredOn) : IDomainEvent;
