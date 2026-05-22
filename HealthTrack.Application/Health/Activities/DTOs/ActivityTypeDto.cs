namespace HealthTrack.Application.Health.Activities.DTOs;

public sealed record ActivityTypeDto(
    int Id,
    string Name,
    float CaloriesPerHour);