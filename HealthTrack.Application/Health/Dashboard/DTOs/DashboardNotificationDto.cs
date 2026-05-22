namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardNotificationDto(
        int Id,
        string Message,
        string Type,
        bool IsRead,
        DateTime CreatedAt);
}
