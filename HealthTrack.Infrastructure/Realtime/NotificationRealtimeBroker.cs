using System.Collections.Concurrent;
using System.Threading.Channels;

namespace HealthTrack.Infrastructure.Realtime;

public sealed class NotificationRealtimeBroker : INotificationRealtimeBroker
{
    private readonly ConcurrentDictionary<int, ConcurrentDictionary<Guid, Channel<NotificationRealtimeMessage>>> _subscriptions = new();

    public (Guid SubscriptionId, ChannelReader<NotificationRealtimeMessage> Reader) Subscribe(int userId)
    {
        var channel = Channel.CreateUnbounded<NotificationRealtimeMessage>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        var id = Guid.NewGuid();

        var userSubs = _subscriptions.GetOrAdd(userId, _ => new ConcurrentDictionary<Guid, Channel<NotificationRealtimeMessage>>());
        userSubs[id] = channel;

        return (id, channel.Reader);
    }

    public void Unsubscribe(int userId, Guid subscriptionId)
    {
        if (!_subscriptions.TryGetValue(userId, out var userSubs))
        {
            return;
        }

        if (userSubs.TryRemove(subscriptionId, out var channel))
        {
            channel.Writer.TryComplete();
        }

        if (userSubs.IsEmpty)
        {
            _subscriptions.TryRemove(userId, out _);
        }
    }

    public ValueTask PublishAsync(int userId, NotificationRealtimeMessage message, CancellationToken cancellationToken)
    {
        if (!_subscriptions.TryGetValue(userId, out var userSubs))
        {
            return ValueTask.CompletedTask;
        }

        foreach (var (_, channel) in userSubs)
        {
            channel.Writer.TryWrite(message);
        }

        return ValueTask.CompletedTask;
    }
}
