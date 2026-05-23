using System.Threading.Channels;

namespace HealthTrack.Infrastructure.Realtime;

public interface INotificationRealtimeBroker
{
    (Guid SubscriptionId, ChannelReader<NotificationRealtimeMessage> Reader) Subscribe(int userId);
    void Unsubscribe(int userId, Guid subscriptionId);
    ValueTask PublishAsync(int userId, NotificationRealtimeMessage message, CancellationToken cancellationToken);
}
