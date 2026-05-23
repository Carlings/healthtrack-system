namespace HealthTrack.Application.Common.Interfaces.Events;

public interface IHealthEventsPublisher
{
    Task PublishHealthRecordCreatedAsync(
        int userId,
        DateTime recordedAt,
        int pulse,
        int systolicBP,
        int diastolicBP,
        CancellationToken cancellationToken);
}
