using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Identity.Auth.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Identity.Auth.Commands;

public record RegisterCommand(
    string Email,
    string Password,
    string Name) : IRequest<AuthResponseDto>;

public class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUserRepository _authRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasherService _passwordHasherService;

    public RegisterCommandHandler(
        IUserRepository authRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasherService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasherService = passwordHasherService;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _authRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException("User with this email already exists.");
        }

        var user = new User
        {
            Email = request.Email,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasherService.HashPassword(
            user,
            request.Password);

        await _authRepository.AddUserAsync(
            user,
            cancellationToken);

        await _authRepository.SaveChangesAsync(
            cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

        return new AuthResponseDto(
            accessToken,
            user.Email,
            user.Name ?? string.Empty);
    }
}