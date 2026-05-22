using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Notifications.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Health.Notifications.Commands;

public sealed record CreateNotificationCommand(
    string Message,
    NotificationType Type)
    : IRequest<NotificationDto>;

public sealed class CreateNotificationCommandHandler
    : IRequestHandler<CreateNotificationCommand, NotificationDto>
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateNotificationCommandHandler(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<NotificationDto> Handle(
        CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var notification = new Notification
        {
            UserId = _currentUser.UserId,
            Message = request.Message,
            Type = request.Type,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(
            notification,
            cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return new NotificationDto(
            notification.Id,
            notification.Message,
            notification.Type.ToString(),
            notification.IsRead,
            notification.CreatedAt);
    }
}