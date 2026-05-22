using HealthTrack.Application.Common.Exceptions;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Notifications.DTOs;
using MediatR;

namespace HealthTrack.Application.Health.Notifications.Queries;

public sealed record GetNotificationByIdQuery(int Id)
    : IRequest<NotificationDto>;

public sealed class GetNotificationByIdQueryHandler
    : IRequestHandler<GetNotificationByIdQuery, NotificationDto>
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationByIdQueryHandler(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<NotificationDto> Handle(
        GetNotificationByIdQuery request,
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

        return new NotificationDto(
            notification.Id,
            notification.Message,
            notification.Type.ToString(),
            notification.IsRead,
            notification.CreatedAt);
    }
}