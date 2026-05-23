using HealthTrack.Application.Common.Interfaces.Realtime;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Domain.Entities;
using HealthTrack.Infrastructure.Services;
using Moq;

namespace HealthTrack.Tests.Health.Notifications;

public sealed class HealthEventsPublisherTests
{
    private Mock<INotificationRepository> _notifications = null!;
    private Mock<INotificationRealtimePublisher> _realtime = null!;
    private HealthEventsPublisher _publisher = null!;

    [SetUp]
    public void SetUp()
    {
        _notifications = new Mock<INotificationRepository>();
        _realtime = new Mock<INotificationRealtimePublisher>();
        _notifications
            .Setup(x => x.GetAllByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        _publisher = new HealthEventsPublisher(_notifications.Object, _realtime.Object);
    }

    [Test]
    public async Task PublishHealthRecordCreatedAsync_WhenPressureHigh_ShouldCreateWarningNotification()
    {
        await _publisher.PublishHealthRecordCreatedAsync(
            2,
            new DateTime(2026, 05, 23, 10, 00, 00, DateTimeKind.Utc),
            80,
            145,
            95,
            CancellationToken.None);

        _notifications.Verify(x => x.AddAsync(
            It.Is<Notification>(n =>
                n.UserId == 2 &&
                n.Message == "High blood pressure detected" &&
                n.Type == NotificationType.Warning &&
                !n.IsRead),
            It.IsAny<CancellationToken>()), Times.Once);
        _notifications.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _realtime.Verify(x => x.PublishCreatedAsync(
            2,
            It.IsAny<int>(),
            "High blood pressure detected",
            "Warning",
            It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task PublishHealthRecordCreatedAsync_WhenPulseHigh_ShouldCreateWarningNotification()
    {
        await _publisher.PublishHealthRecordCreatedAsync(
            2,
            new DateTime(2026, 05, 23, 10, 00, 00, DateTimeKind.Utc),
            120,
            120,
            80,
            CancellationToken.None);

        _notifications.Verify(x => x.AddAsync(
            It.Is<Notification>(n =>
                n.Message == "High heart rate detected" &&
                n.Type == NotificationType.Warning),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task PublishHealthRecordCreatedAsync_WhenNormalMetrics_ShouldNotCreateNotifications()
    {
        await _publisher.PublishHealthRecordCreatedAsync(
            2,
            new DateTime(2026, 05, 23, 10, 00, 00, DateTimeKind.Utc),
            72,
            120,
            80,
            CancellationToken.None);

        _notifications.Verify(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        _notifications.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _realtime.Verify(x => x.PublishCreatedAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<DateTime>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task PublishHealthRecordCreatedAsync_WhenSameNotificationWithinCooldown_ShouldSkipDuplicate()
    {
        var now = DateTime.UtcNow;
        _notifications
            .Setup(x => x.GetAllByUserIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new Notification
                {
                    UserId = 2,
                    Message = "High blood pressure detected",
                    Type = NotificationType.Warning,
                    CreatedAt = now.AddSeconds(-10),
                    IsRead = false
                }
            ]);

        await _publisher.PublishHealthRecordCreatedAsync(
            2,
            now,
            80,
            150,
            95,
            CancellationToken.None);

        _notifications.Verify(x => x.AddAsync(It.IsAny<Notification>(), It.IsAny<CancellationToken>()), Times.Never);
        _notifications.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task PublishHealthRecordCreatedAsync_WhenExistingNotificationHasFutureCreatedAt_ShouldCreateNewNotification()
    {
        var now = DateTime.UtcNow;
        _notifications
            .Setup(x => x.GetAllByUserIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            [
                new Notification
                {
                    UserId = 2,
                    Message = "High heart rate detected",
                    Type = NotificationType.Warning,
                    CreatedAt = now.AddMinutes(20),
                    IsRead = false
                }
            ]);

        await _publisher.PublishHealthRecordCreatedAsync(
            2,
            now,
            150,
            120,
            80,
            CancellationToken.None);

        _notifications.Verify(x => x.AddAsync(
            It.Is<Notification>(n => n.Message == "High heart rate detected"),
            It.IsAny<CancellationToken>()), Times.Once);
        _notifications.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
