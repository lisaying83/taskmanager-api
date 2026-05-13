using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Tasks.Commands.AssignTask;
using TaskManager.Application.Tasks.Commands.CompleteTask;
using TaskManager.Application.Tasks.Commands.CreateTask;
using TaskManager.Application.Tasks.Queries.GetTask;
using TaskManager.Application.Tasks.Queries.GetTasks;
using TaskManager.Domain.Enums;

namespace TaskManager.Api.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks")
            .WithTags("Tasks")
            .RequireAuthorization();

        group.MapPost("/", async (
            CreateTaskRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = GetUserId(user);
            var command = new CreateTaskCommand(
                request.Title, request.Description, request.Priority, userId, request.DueDate);
            var result = await sender.Send(command, ct);
            return Results.Created($"/api/tasks/{result.TaskId}", result);
        })
        .WithName("CreateTask")
        .WithSummary("Create a new task");

        group.MapGet("/", async (
            ISender sender,
            ClaimsPrincipal user,
            [FromQuery] WorkTaskStatus? status,
            CancellationToken ct) =>
        {
            var userId = GetUserId(user);
            var result = await sender.Send(new GetTasksQuery(userId, status), ct);
            return Results.Ok(result);
        })
        .WithName("GetTasks")
        .WithSummary("List tasks for the authenticated user");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetTaskQuery(id), ct);
            return result is null ? Results.NotFound() : Results.Ok(result);
        })
        .WithName("GetTask")
        .WithSummary("Get a task by id");

        group.MapPost("/{id:guid}/complete", async (
            Guid id,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = GetUserId(user);
            await sender.Send(new CompleteTaskCommand(id, userId), ct);
            return Results.NoContent();
        })
        .WithName("CompleteTask")
        .WithSummary("Mark a task as done");

        group.MapPost("/{id:guid}/assign", async (
            Guid id,
            AssignTaskRequest request,
            ISender sender,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = GetUserId(user);
            await sender.Send(new AssignTaskCommand(id, request.AssignedToUserId, userId), ct);
            return Results.NoContent();
        })
        .WithName("AssignTask")
        .WithSummary("Assign a task to a user");
    }

    private static Guid GetUserId(ClaimsPrincipal user)
    {
        var sub = user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
               ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(sub!);
    }
}

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateTime? DueDate);

public sealed record AssignTaskRequest(Guid AssignedToUserId);
