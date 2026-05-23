using HealthTrack.Application.Common.Interfaces.Events;
using HealthTrack.Application.Common.Interfaces.Realtime;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;

namespace HealthTrack.Infrastructure.Services;

public sealed class HealthEventsPublisher : IHealthEventsPublisher
{
    private static readonly TimeSpan DuplicateCooldown = TimeSpan.FromMinutes(1);

    private readonly INotificationRepository _notifications;
    private readonly INotificationRealtimePublisher _realtimePublisher;

    public HealthEventsPublisher(
        INotificationRepository notifications,
        INotificationRealtimePublisher realtimePublisher)
    {
        _notifications = notifications;
        _realtimePublisher = realtimePublisher;
    }

    public async Task PublishHealthRecordCreatedAsync(
        int userId,
        DateTime recordedAt,
        int pulse,
        int systolicBP,
        int diastolicBP,
        CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var toCreate = new List<(string Message, NotificationType Type)>();

        if (systolicBP >= 140 || diastolicBP >= 90)
        {
            toCreate.Add(("High blood pressure detected", NotificationType.Warning));
        }

        if (pulse >= 110)
        {
            toCreate.Add(("High heart rate detected", NotificationType.Warning));
        }

        if (toCreate.Count == 0)
        {
            return;
        }

        var existing = await _notifications.GetAllByUserIdAsync(
            userId,
            cancellationToken);

        var hasNewNotifications = false;

        var created = new List<Notification>();

        foreach (var candidate in toCreate)
        {
            if (ShouldSkipByCooldown(existing, candidate.Message, candidate.Type, nowUtc))
            {
                continue;
            }

            var notification = new Notification
            {
                UserId = userId,
                Message = candidate.Message,
                Type = candidate.Type,
                IsRead = false,
                CreatedAt = nowUtc
            };

            await _notifications.AddAsync(notification, cancellationToken);
            created.Add(notification);

            hasNewNotifications = true;
        }

        if (hasNewNotifications)
        {
            await _notifications.SaveChangesAsync(cancellationToken);

            foreach (var notification in created)
            {
                await _realtimePublisher.PublishCreatedAsync(
                    userId,
                    notification.Id,
                    notification.Message,
                    notification.Type.ToString(),
                    notification.CreatedAt,
                    cancellationToken);
            }
        }
    }

    private static bool ShouldSkipByCooldown(
        IReadOnlyList<Notification> existing,
        string message,
        NotificationType type,
        DateTime currentMomentUtc)
    {
        var latestSame = existing
            .Where(x =>
                x.Type == type &&
                string.Equals(
                    x.Message?.Trim(),
                    message.Trim(),
                    StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault();

        if (latestSame is null)
        {
            return false;
        }

        if (latestSame.CreatedAt > currentMomentUtc)
        {
            return false;
        }

        return currentMomentUtc - latestSame.CreatedAt < DuplicateCooldown;
    }
}
