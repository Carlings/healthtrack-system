namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record WeightTrendPointDto(
        DateTime RecordedAt,
        float Weight);
}
