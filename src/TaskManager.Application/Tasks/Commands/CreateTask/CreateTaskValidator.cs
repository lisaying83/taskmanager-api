using FluentValidation;
using TaskManager.Domain.ValueObjects;

namespace TaskManager.Application.Tasks.Commands.CreateTask;

public sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(TaskTitle.MaxLength).WithMessage($"Title cannot exceed {TaskTitle.MaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(TaskDescription.MaxLength)
            .When(x => x.Description is not null)
            .WithMessage($"Description cannot exceed {TaskDescription.MaxLength} characters.");

        RuleFor(x => x.CreatedByUserId)
            .NotEmpty().WithMessage("CreatedByUserId is required.");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.")
            .When(x => x.DueDate.HasValue);
    }
}
