namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardOverviewDto(
        DashboardStatsDto? Stats,
        DashboardGoalDto? Goal,
        IReadOnlyList<WeightTrendPointDto> WeightTrend,
        IReadOnlyList<DashboardRecordDto> LatestRecords,
        IReadOnlyList<DashboardActivityDto> LatestActivities,
        IReadOnlyList<DashboardNotificationDto> LatestNotifications,
        int UnreadNotificationsCount);
}
