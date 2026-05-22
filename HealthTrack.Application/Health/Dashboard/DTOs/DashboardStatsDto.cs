namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardStatsDto(
        DateTime RecordedAt,
        float Weight,
        int Pulse,
        float Temperature,
        int SystolicBP,
        int DiastolicBP,
        int Steps,
        float SleepHours);
}
