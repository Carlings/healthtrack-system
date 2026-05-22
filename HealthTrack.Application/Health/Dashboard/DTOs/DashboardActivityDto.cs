namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardActivityDto(
        int Id,
        string ActivityType,
        int DurationMinutes,
        int CaloriesBurned,
        DateTime ActivityDate);
}
