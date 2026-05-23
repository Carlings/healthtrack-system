using HealthTrack.Domain.Entities;

namespace HealthTrack.Tests.Common;

internal static class TestDataFactory
{
    public static User CreateUser(
        int id = 1,
        string email = "john@test.com",
        string name = "John",
        string passwordHash = "hashed-password",
        int tokenVersion = 0,
        string? avatarUrl = null)
    {
        return new User
        {
            Id = id,
            Email = email,
            Name = name,
            PasswordHash = passwordHash,
            CreatedAt = new DateTime(2026, 05, 01, 12, 00, 00, DateTimeKind.Utc),
            BirthDate = DateOnly.FromDateTime(new DateTime(2000, 01, 01)),
            Height = 180,
            Gender = Gender.Male,
            TokenVersion = tokenVersion,
            AvatarUrl = avatarUrl
        };
    }

    public static RefreshToken CreateRefreshToken(
        User user,
        string token = "refresh-token",
        bool isRevoked = false,
        DateTime? expiresAt = null)
    {
        return new RefreshToken
        {
            Id = 1,
            UserId = user.Id,
            User = user,
            Token = token,
            IsRevoked = isRevoked,
            CreatedAt = new DateTime(2026, 05, 22, 10, 00, 00, DateTimeKind.Utc),
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
        };
    }

    public static HealthRecord CreateHealthRecord(
        int id,
        DateTime recordedAt,
        float weight,
        int pulse,
        float temperature,
        int systolicBp,
        int diastolicBp,
        int steps,
        float sleepHours,
        int userId = 1)
    {
        return new HealthRecord
        {
            Id = id,
            UserId = userId,
            RecordedAt = recordedAt,
            Weight = weight,
            Pulse = pulse,
            Temperature = temperature,
            SystolicBP = systolicBp,
            DiastolicBP = diastolicBp,
            Steps = steps,
            SleepHours = sleepHours
        };
    }

    public static Goal CreateGoal(
        int id,
        int userId,
        float? targetWeight,
        int? targetSteps,
        DateTime createdAt)
    {
        return new Goal
        {
            Id = id,
            UserId = userId,
            TargetWeight = targetWeight,
            TargetSteps = targetSteps,
            CreatedAt = createdAt
        };
    }

    public static ActivityType CreateActivityType(
        int id,
        string name,
        float caloriesPerHour)
    {
        return new ActivityType
        {
            Id = id,
            Name = name,
            CaloriesPerHour = caloriesPerHour
        };
    }

    public static UserActivity CreateUserActivity(
        int id,
        User user,
        ActivityType activityType,
        int durationMinutes,
        int caloriesBurned,
        DateTime activityDate)
    {
        return new UserActivity
        {
            Id = id,
            UserId = user.Id,
            User = user,
            ActivityTypeId = activityType.Id,
            ActivityType = activityType,
            DurationMinutes = durationMinutes,
            CaloriesBurned = caloriesBurned,
            ActivityDate = activityDate
        };
    }

    public static Notification CreateNotification(
        int id,
        User user,
        NotificationType type,
        string message,
        bool isRead,
        DateTime createdAt)
    {
        return new Notification
        {
            Id = id,
            UserId = user.Id,
            User = user,
            Type = type,
            Message = message,
            IsRead = isRead,
            CreatedAt = createdAt
        };
    }
}