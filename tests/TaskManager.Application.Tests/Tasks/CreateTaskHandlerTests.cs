using FluentAssertions;
using NSubstitute;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Tests.Tasks;

public class CreateTaskHandlerTests
{
    private readonly ITaskRepository _taskRepo = Substitute.For<ITaskRepository>();
    private readonly IUserRepository _userRepo = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();

    private CreateTaskHandler CreateHandler() => new(_taskRepo, _userRepo, _uow);

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_ReturnsCreatedTask()
    {
        var userId = Guid.NewGuid();
        var user = User.Create("alice@test.com", "Alice", "hash");
        _userRepo.GetByIdAsync(userId, default).Returns(user);

        var command = new CreateTaskCommand("Fix bug", "Description", TaskPriority.High, userId, null);
        var result = await CreateHandler().Handle(command, default);

        result.Title.Should().Be("Fix bug");
        result.Status.Should().Be(WorkTaskStatus.Todo);
        await _taskRepo.Received(1).AddAsync(Arg.Any<TaskItem>(), default);
        await _uow.Received(1).SaveChangesAsync(default);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UserNotFound_ThrowsDomainException()
    {
        _userRepo.GetByIdAsync(Arg.Any<Guid>(), default).Returns((User?)null);

        var command = new CreateTaskCommand("Fix bug", null, TaskPriority.Low, Guid.NewGuid(), null);
        var act = async () => await CreateHandler().Handle(command, default);

        await act.Should().ThrowAsync<Domain.Exceptions.DomainException>()
            .WithMessage("*User not found*");
    }
}
