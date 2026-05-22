using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.Notifications.Commands;

public sealed record DeleteNotificationCommand(int Id) : IRequest;

public sealed class DeleteNotificationCommandHandler
    : IRequestHandler<DeleteNotificationCommand>
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public DeleteNotificationCommandHandler(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        DeleteNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(
            request.Id,
            _currentUser.UserId,
            cancellationToken);

        if (notification is null)
        {
            throw new NotFoundException(
                "Notification",
                request.Id);
        }

        _repository.Delete(notification);

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}