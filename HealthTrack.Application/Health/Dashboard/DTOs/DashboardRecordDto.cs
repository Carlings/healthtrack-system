namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardRecordDto(
        int Id,
        DateTime RecordedAt,
        float Weight,
        int Steps);
}
