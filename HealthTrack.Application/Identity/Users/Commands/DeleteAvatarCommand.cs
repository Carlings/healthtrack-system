using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Common.Interfaces.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthTrack.Application.Identity.Users.Commands;

public sealed record DeleteAvatarCommand : IRequest;

public sealed class DeleteAvatarCommandHandler
    : IRequestHandler<DeleteAvatarCommand>
{
    private readonly ILogger<DeleteAvatarCommandHandler> _logger;

    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;
    private readonly IAvatarStorageService _storage;

    public DeleteAvatarCommandHandler(
        IUserRepository users,
        ICurrentUserService currentUser,
        IAvatarStorageService storage,
        ILogger<DeleteAvatarCommandHandler> logger)
    {
        _users = users;
        _currentUser = currentUser;
        _storage = storage;
        _logger = logger;
    }

    public async Task Handle(
        DeleteAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetTrackedByIdAsync(
            _currentUser.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", _currentUser.UserId);
        }

        if (string.IsNullOrWhiteSpace(user.AvatarUrl))
        {
            return;
        }

        var oldAvatarUrl = user.AvatarUrl;
        user.AvatarUrl = null;

        await _users.SaveChangesAsync(cancellationToken);

        try
        {
            await _storage.DeleteAsync(oldAvatarUrl, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to delete avatar file for user {UserId}",
                user.Id);
        }
    }
}