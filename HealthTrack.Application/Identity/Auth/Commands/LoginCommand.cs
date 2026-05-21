using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Identity.Auth.DTOs;
using MediatR;

namespace HealthTrack.Application.Identity.Auth.Commands
{
    public record LoginCommand(
    string Email,
    string Password) : IRequest<AuthResponseDto>;

    public class LoginCommandHandler
    : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IUserRepository _repository;
        private readonly IJwtTokenService _tokenService;
        private readonly IPasswordHasherService _passwordHasher;

        public LoginCommandHandler(
            IUserRepository repository,
            IJwtTokenService tokenService,
            IPasswordHasherService passwordHasher)
        {
            _repository = repository;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task<AuthResponseDto> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var user = await _repository.GetByEmailAsync(
                request.Email,
                cancellationToken);

            if (user is null)
            {
                throw new InvalidOperationException(
                    "Invalid credentials.");
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (!isPasswordValid)
            {
                throw new InvalidOperationException(
                    "Invalid credentials.");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);

            return new AuthResponseDto(
                accessToken,
                user.Email,
                user.Name ?? string.Empty);
        }
    }
}
