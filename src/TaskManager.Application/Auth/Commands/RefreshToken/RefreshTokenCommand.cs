using MediatR;

namespace TaskManager.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResult>;

public sealed record RefreshTokenResult(string AccessToken, string RefreshToken);
