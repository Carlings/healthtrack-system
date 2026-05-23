using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Common.Interfaces.Storage;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HealthTrack.Application.Identity.Users.Commands;

public sealed record UploadAvatarCommand(
    Stream Stream,
    string FileName,
    long Length) : IRequest<string>;

public sealed class UploadAvatarCommandHandler
    : IRequestHandler<UploadAvatarCommand, string>
{
    private readonly ILogger<UploadAvatarCommandHandler> _logger;

    private readonly IUserRepository _users;
    private readonly ICurrentUserService _currentUser;
    private readonly IAvatarStorageService _storage;

    public UploadAvatarCommandHandler(
        IUserRepository users,
        ICurrentUserService currentUser,
        IAvatarStorageService storage,
        ILogger<UploadAvatarCommandHandler> logger)
    {
        _users = users;
        _currentUser = currentUser;
        _storage = storage;
        _logger = logger;
    }

    public async Task<string> Handle(
        UploadAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _users.GetTrackedByIdAsync(
            _currentUser.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException("User", _currentUser.UserId);
        }

        var oldAvatarUrl = user.AvatarUrl;

        var newAvatarUrl = await _storage.UploadAsync(
            request.Stream,
            request.FileName,
            cancellationToken);

        user.AvatarUrl = newAvatarUrl;

        try
        {
            await _users.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            await _storage.DeleteAsync(newAvatarUrl, cancellationToken);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldAvatarUrl))
        {
            try
            {
                await _storage.DeleteAsync(oldAvatarUrl, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogWarning(
                        ex,
                        "Failed to delete old avatar for user {UserId}",
                        user.Id);
            }
        }

        return newAvatarUrl;
    }
}