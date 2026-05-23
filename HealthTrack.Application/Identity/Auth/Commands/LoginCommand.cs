using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Identity.Auth.Commands
{
    public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;

    public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IRefreshTokenService _refreshTokenService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IJwtTokenService tokenService,
            IPasswordHasherService passwordHasher,
            IRefreshTokenService refreshTokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<AuthResponseDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (user is null)
            {
                throw new UnauthorizedAccessException(
                    "Invalid credentials.");
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException(
                    "Invalid credentials.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            var refreshTokenValue = _refreshTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenValue,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddRefreshTokenAsync(
                refreshToken,
                cancellationToken);

            await _userRepository.SaveChangesAsync(
                cancellationToken);

            return new AuthResponseDto(
                accessToken,
                refreshTokenValue,
                user.Email,
                user.Name ?? string.Empty);
        }
    }
}
