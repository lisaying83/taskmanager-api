using MediatR;
using TaskManager.Application.Auth.Commands.Login;
using TaskManager.Application.Auth.Commands.RefreshToken;

namespace TaskManager.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/login", async (LoginCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithSummary("Authenticate and receive JWT tokens");

        group.MapPost("/refresh", async (RefreshTokenCommand command, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return Results.Ok(result);
        })
        .WithName("RefreshToken")
        .WithSummary("Rotate refresh token and get new access token");
    }
}
