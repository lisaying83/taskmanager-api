using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.ValueObjects;

public sealed class TaskDescription : IEquatable<TaskDescription>
{
    public const int MaxLength = 2000;

    public string Value { get; }

    private TaskDescription(string value) => Value = value;

    public static TaskDescription Create(string? value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (trimmed.Length > MaxLength)
            throw new DomainException($"Task description cannot exceed {MaxLength} characters.");

        return new TaskDescription(trimmed);
    }

    public bool Equals(TaskDescription? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is TaskDescription d && Equals(d);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;
}
