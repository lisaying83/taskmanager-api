namespace TaskManager.Domain.Events;

public sealed record TaskCreatedEvent(
    Guid Id,
    Guid TaskId,
    string Title,
    Guid CreatedByUserId,
    DateTime OccurredOn) : IDomainEvent;
