using MediatR;

namespace TaskManager.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public sealed record LoginResult(string AccessToken, string RefreshToken, Guid UserId);
