namespace HealthTrack.Application.Health.Notifications.DTOs;

public sealed record NotificationDto(
    int Id,
    string Message,
    string Type,
    bool IsRead,
    DateTime CreatedAt);