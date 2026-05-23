using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Identity.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;

public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResponseDto> Handle(
        RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _userRepository.GetRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null)
        {
            throw new InvalidOperationException(
                "Invalid refresh token.");
        }

        if (refreshToken.IsRevoked)
        {
            throw new InvalidOperationException(
                "Refresh token revoked.");
        }

        if (refreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidOperationException(
                "Refresh token expired.");
        }

        refreshToken.IsRevoked = true;

        var accessToken =
            _jwtTokenService.GenerateAccessToken(
                refreshToken.User);

        var newRefreshTokenValue =
            _refreshTokenService.GenerateRefreshToken();

        var newRefreshToken = new RefreshToken
        {
            UserId = refreshToken.UserId,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddRefreshTokenAsync(
            newRefreshToken,
            cancellationToken);

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        return new AuthResponseDto(
            accessToken,
            newRefreshTokenValue,
            refreshToken.User.Email,
            refreshToken.User.Name ?? string.Empty);
    }
}