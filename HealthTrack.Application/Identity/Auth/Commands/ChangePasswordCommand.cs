using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Identity.Users.Commands;

public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword)
    : IRequest;

public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;
    private readonly IPasswordHasherService _passwordHasher;

    public ChangePasswordCommandHandler(
        IUserRepository users,
        ICurrentUserService currentUser,
        IPasswordHasherService passwordHasher)
    {
        _users = users;
        _currentUser = currentUser;
        _passwordHasher = passwordHasher;
    }

    public async Task Handle(
        ChangePasswordCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetTrackedByIdAsync(
            _currentUser.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "User",
                _currentUser.UserId);
        }

        var isPasswordValid =
            _passwordHasher.VerifyPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword);

        if (!isPasswordValid)
        {
            throw new InvalidOperationException(
                "Current password is incorrect.");
        }

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                request.NewPassword);

        user.TokenVersion++;

        await _users.SaveChangesAsync(
            cancellationToken);
    }
}