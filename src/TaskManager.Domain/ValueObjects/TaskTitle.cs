using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.ValueObjects;

public sealed class TaskTitle : IEquatable<TaskTitle>
{
    public const int MaxLength = 200;

    public string Value { get; }

    private TaskTitle(string value) => Value = value;

    public static TaskTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Task title cannot be empty.");

        if (value.Length > MaxLength)
            throw new DomainException($"Task title cannot exceed {MaxLength} characters.");

        return new TaskTitle(value.Trim());
    }

    public bool Equals(TaskTitle? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is TaskTitle t && Equals(t);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
