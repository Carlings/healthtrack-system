using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Identity.Auth.Commands;

public sealed record LogoutCommand(string RefreshToken) : IRequest;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUserRepository _userRepository;

    public LogoutCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Handle(
        LogoutCommand request,
        CancellationToken cancellationToken)
    {
        var refreshToken = await _userRepository.GetRefreshTokenAsync(
            request.RefreshToken,
            cancellationToken);

        if (refreshToken is null)
        {
            return;
        }

        refreshToken.IsRevoked = true;

        var user = refreshToken.User;

        user.TokenVersion++;

        await _userRepository.SaveChangesAsync(
            cancellationToken);
    }
}