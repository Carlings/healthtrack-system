using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Dashboard.DTOs;
using HealthTrack.Domain.Entities;
using MediatR;

namespace HealthTrack.Application.Health.Dashboard.Queries;

public sealed record GetDashboardOverviewQuery : IRequest<DashboardOverviewDto>;

public sealed class GetDashboardOverviewQueryHandler
    : IRequestHandler<GetDashboardOverviewQuery, DashboardOverviewDto>
{
    private readonly IHealthRecordRepository _healthRecords;
    private readonly IGoalRepository _goals;
    private readonly IUserActivityRepository _activities;
    private readonly INotificationRepository _notifications;
    private readonly ICurrentUserService _currentUser;

    public GetDashboardOverviewQueryHandler(
        IHealthRecordRepository healthRecords,
        IGoalRepository goals,
        IUserActivityRepository activities,
        INotificationRepository notifications,
        ICurrentUserService currentUser)
    {
        _healthRecords = healthRecords;
        _goals = goals;
        _activities = activities;
        _notifications = notifications;
        _currentUser = currentUser;
    }

    public async Task<DashboardOverviewDto> Handle(
        GetDashboardOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var healthRecords = await _healthRecords.GetByUserIdAsync(
            userId,
            from: null,
            to: null,
            cancellationToken);

        var goals = await _goals.GetByUserIdAsync(
            userId,
            cancellationToken);

        var activities = await _activities.GetByUserIdAsync(
            userId,
            cancellationToken);

        var unreadNotifications = await _notifications.GetUnreadByUserIdAsync(
            userId,
            cancellationToken);

        var latestRecord = healthRecords.FirstOrDefault();
        var stats = latestRecord is null
            ? null
            : new DashboardStatsDto(
                latestRecord.RecordedAt,
                latestRecord.Weight,
                latestRecord.Pulse,
                latestRecord.Temperature,
                latestRecord.SystolicBP,
                latestRecord.DiastolicBP,
                latestRecord.Steps,
                latestRecord.SleepHours);

        var trendWindow = healthRecords
            .Take(7)
            .OrderBy(x => x.RecordedAt)
            .ToList();

        var latestRecords = healthRecords
            .Take(3)
            .Select(x => new DashboardRecordDto(
                x.Id,
                x.RecordedAt,
                x.Weight,
                x.Steps))
            .ToList();

        var latestActivities = activities
            .Take(4)
            .Select(x => new DashboardActivityDto(
                x.Id,
                x.ActivityType.Name,
                x.DurationMinutes,
                x.CaloriesBurned,
                x.ActivityDate))
            .ToList();

        var latestNotifications = unreadNotifications
            .Take(3)
            .Select(x => new DashboardNotificationDto(
                x.Id,
                x.Message,
                x.Type.ToString(),
                x.IsRead,
                x.CreatedAt))
            .ToList();

        var currentGoal = goals.FirstOrDefault();
        var goalDto = currentGoal is null
            ? null
            : BuildGoalDto(currentGoal, trendWindow, latestRecord);

        return new DashboardOverviewDto(
            stats,
            goalDto,
            [.. trendWindow.Select(x => new WeightTrendPointDto(x.RecordedAt, x.Weight))],
            latestRecords,
            latestActivities,
            latestNotifications,
            unreadNotifications.Count);
    }

    private static DashboardGoalDto BuildGoalDto(
        Goal goal,
        IReadOnlyList<HealthRecord> trendWindow,
        HealthRecord? latestRecord)
    {
        float? weightProgressPercent = null;
        float? currentWeight = latestRecord?.Weight;
        float? weightGap = null;

        if (goal.TargetWeight.HasValue && latestRecord is not null)
        {
            currentWeight = latestRecord.Weight;
            weightGap = latestRecord.Weight - goal.TargetWeight.Value;

            var baselineRecord = trendWindow.FirstOrDefault();
            if (baselineRecord is not null)
            {
                weightProgressPercent = CalculateWeightProgress(
                    baselineRecord.Weight,
                    latestRecord.Weight,
                    goal.TargetWeight.Value);
            }
        }

        int? currentSteps = latestRecord?.Steps;
        int? stepsGap = null;
        float? stepsProgressPercent = null;

        if (goal.TargetSteps.HasValue && latestRecord is not null)
        {
            currentSteps = latestRecord.Steps;
            stepsGap = goal.TargetSteps.Value - latestRecord.Steps;

            stepsProgressPercent = CalculateStepsProgress(
                latestRecord.Steps,
                goal.TargetSteps.Value);
        }

        return new DashboardGoalDto(
            goal.Id,
            goal.TargetWeight,
            goal.TargetSteps,
            goal.CreatedAt,
            weightProgressPercent,
            stepsProgressPercent,
            currentWeight,
            currentSteps,
            weightGap,
            stepsGap);
    }

    private static float CalculateWeightProgress(
        float baselineWeight,
        float currentWeight,
        float targetWeight)
    {
        if (baselineWeight == targetWeight)
        {
            return 100f;
        }

        float progress;

        if (targetWeight < baselineWeight)
        {
            var totalDelta = baselineWeight - targetWeight;
            var achievedDelta = baselineWeight - currentWeight;

            progress = (achievedDelta / totalDelta) * 100f;
        }
        else
        {
            var totalDelta = targetWeight - baselineWeight;
            var achievedDelta = currentWeight - baselineWeight;

            progress = (achievedDelta / totalDelta) * 100f;
        }

        return Math.Clamp(progress, 0f, 100f);
    }

    private static float CalculateStepsProgress(int currentSteps, int targetSteps)
    {
        if (targetSteps <= 0)
        {
            return 0f;
        }

        return Math.Clamp((currentSteps / (float)targetSteps) * 100f, 0f, 100f);
    }
}