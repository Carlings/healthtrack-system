namespace HealthTrack.Application.Health.Activities.DTOs;

public sealed record UserActivityDto(
    int Id,
    int ActivityTypeId,
    string ActivityType,
    int DurationMinutes,
    int CaloriesBurned,
    DateTime ActivityDate);