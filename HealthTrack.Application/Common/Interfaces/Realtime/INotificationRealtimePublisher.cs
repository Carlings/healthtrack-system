namespace HealthTrack.Application.Common.Interfaces.Realtime;

public interface INotificationRealtimePublisher
{
    Task PublishCreatedAsync(
        int userId,
        int notificationId,
        string message,
        string type,
        DateTime createdAt,
        CancellationToken cancellationToken);
}
