namespace HealthTrack.Application.Health.Dashboard.DTOs
{
    public sealed record DashboardGoalDto(
        int Id,
        float? TargetWeight,
        int? TargetSteps,
        DateTime CreatedAt,
        float? WeightProgressPercent,
        float? StepsProgressPercent,
        float? CurrentWeight,
        int? CurrentSteps,
        float? WeightGap,
        int? StepsGap);
}
