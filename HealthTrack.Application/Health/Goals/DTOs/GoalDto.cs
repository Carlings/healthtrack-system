namespace HealthTrack.Application.Health.Goals.DTOs
{
    public sealed record GoalDto(
        int Id,
        float? TargetWeight,
        int? TargetSteps,
        DateTime CreatedAt);
}
