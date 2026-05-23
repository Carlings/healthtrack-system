using HealthTrack.Application.Common.Interfaces.Realtime;
using HealthTrack.Infrastructure.Realtime;

namespace HealthTrack.Infrastructure.Services;

public sealed class NotificationRealtimePublisher : INotificationRealtimePublisher
{
    private readonly INotificationRealtimeBroker _broker;

    public NotificationRealtimePublisher(INotificationRealtimeBroker broker)
    {
        _broker = broker;
    }

    public Task PublishCreatedAsync(
        int userId,
        int notificationId,
        string message,
        string type,
        DateTime createdAt,
        CancellationToken cancellationToken)
    {
        return _broker.PublishAsync(
            userId,
            new NotificationRealtimeMessage(
                "notification.created",
                notificationId,
                message,
                type,
                createdAt),
            cancellationToken).AsTask();
    }
}
