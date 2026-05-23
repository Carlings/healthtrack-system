using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
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
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IRefreshTokenService _refreshTokenService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasherService passwordHasherService,
        IRefreshTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasherService = passwordHasherService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<AuthResponseDto> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException("User with this email already exists.");
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

        await _userRepository.AddAsync(
            user,
            cancellationToken);

        await _userRepository.SaveChangesAsync(
            cancellationToken);

        var accessToken = _jwtTokenService.GenerateAccessToken(user);

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