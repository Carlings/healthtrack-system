namespace HealthTrack.Infrastructure.Realtime;

public sealed record NotificationRealtimeMessage(
    string Event,
    int NotificationId,
    string Message,
    string Type,
    DateTime CreatedAt);
