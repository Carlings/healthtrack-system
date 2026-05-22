namespace HealthTrack.Application.Health.HealthRecords.DTOs
{
    public sealed record HealthRecordDto(
        int Id,
        DateTime RecordedAt,
        float Weight,
        int Pulse,
        float Temperature,
        int SystolicBP,
        int DiastolicBP,
        int Steps,
        float SleepHours);
}
