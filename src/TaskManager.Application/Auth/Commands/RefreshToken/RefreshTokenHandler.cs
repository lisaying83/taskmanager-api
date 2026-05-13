using MediatR;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Auth.Commands.RefreshToken;

public sealed class RefreshTokenHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IUnitOfWork unitOfWork) : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var userId = jwtTokenService.GetUserIdFromExpiredToken(request.AccessToken)
            ?? throw new DomainException("Invalid access token.");

        var user = await userRepository.GetByIdAsync(userId, ct)
            ?? throw new DomainException("User not found.");

        if (!user.HasValidRefreshToken(request.RefreshToken))
            throw new DomainException("Invalid or expired refresh token.");

        var newAccessToken = jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();

        user.SetRefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7));
        userRepository.Update(user);
        await unitOfWork.SaveChangesAsync(ct);

        return new RefreshTokenResult(newAccessToken, newRefreshToken);
    }
}
