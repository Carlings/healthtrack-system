using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using MediatR;

namespace HealthTrack.Application.Health.Notifications.Commands;

public sealed record MarkNotificationAsReadCommand(int Id) : IRequest;

public sealed class MarkNotificationAsReadCommandHandler
    : IRequestHandler<MarkNotificationAsReadCommand>
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationAsReadCommandHandler(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task Handle(
        MarkNotificationAsReadCommand request,
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

        notification.IsRead = true;

        await _repository.SaveChangesAsync(
            cancellationToken);
    }
}