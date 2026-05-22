using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Notifications.DTOs;
using MediatR;
using System.Linq;

namespace HealthTrack.Application.Health.Notifications.Queries;

public sealed record GetUnreadNotificationsQuery
    : IRequest<IReadOnlyList<NotificationDto>>;

public sealed class GetUnreadNotificationsQueryHandler
    : IRequestHandler<GetUnreadNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    private readonly INotificationRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadNotificationsQueryHandler(
        INotificationRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<NotificationDto>> Handle(
        GetUnreadNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetUnreadByUserIdAsync(
            _currentUser.UserId,
            cancellationToken);

        return [.. notifications
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new NotificationDto(
                x.Id,
                x.Message,
                x.Type.ToString(),
                x.IsRead,
                x.CreatedAt))];
    }
}