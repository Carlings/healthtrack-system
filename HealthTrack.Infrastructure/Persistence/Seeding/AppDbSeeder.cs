using Bogus;
using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Seeding;

public static class AppDbSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        IPasswordHasherService passwordHasherService)
    {
        await context.Database.MigrateAsync();

        Randomizer.Seed = new Random(42);
        var faker = new Faker("en");

        if (!await context.ActivityTypes.AnyAsync())
        {
            var activityTypes = new List<ActivityType>
            {
                new() { Name = "Running", CaloriesPerHour = 600 },
                new() { Name = "Walking", CaloriesPerHour = 250 },
                new() { Name = "Cycling", CaloriesPerHour = 500 },
                new() { Name = "Swimming", CaloriesPerHour = 650 },
                new() { Name = "Gym", CaloriesPerHour = 400 }
            };

            context.ActivityTypes.AddRange(activityTypes);
            await context.SaveChangesAsync();
        }

        var targetUser = await context.Users
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync();

        if (targetUser is null)
        {
            targetUser = new User
            {
                Email = "demo@healthtrack.local",
                Name = "Demo User",
                BirthDate = DateOnly.FromDateTime(
                    faker.Date.Past(25, DateTime.Today.AddYears(-18))),
                Height = faker.Random.Int(160, 195),
                Gender = faker.PickRandom(Gender.Male, Gender.Female),
                CreatedAt = DateTime.UtcNow
            };

            targetUser.PasswordHash = passwordHasherService.HashPassword(
                targetUser,
                "DemoPassword123!");

            context.Users.Add(targetUser);
            await context.SaveChangesAsync();
        }

        if (!await context.HealthRecords.AnyAsync(x => x.UserId == targetUser.Id))
        {
            var records = new Faker<HealthRecord>()
                .RuleFor(x => x.UserId, _ => targetUser.Id)
                .RuleFor(x => x.RecordedAt, f => f.Date.Recent(30))
                .RuleFor(x => x.Weight, f => f.Random.Float(55, 110))
                .RuleFor(x => x.Pulse, f => f.Random.Int(55, 95))
                .RuleFor(x => x.Temperature, f => f.Random.Float(35.8f, 37.5f))
                .RuleFor(x => x.SystolicBP, f => f.Random.Int(110, 145))
                .RuleFor(x => x.DiastolicBP, f => f.Random.Int(65, 95))
                .RuleFor(x => x.Steps, f => f.Random.Int(2000, 12000))
                .RuleFor(x => x.SleepHours, f => f.Random.Float(5f, 9f))
                .Generate(10);

            context.HealthRecords.AddRange(records);
            await context.SaveChangesAsync();
        }

        if (!await context.Goals.AnyAsync(x => x.UserId == targetUser.Id))
        {
            var goals = new List<Goal>
            {
                new()
                {
                    UserId = targetUser.Id,
                    TargetWeight = 75,
                    TargetSteps = 10000,
                    CreatedAt = DateTime.UtcNow.AddDays(-21)
                },
                new()
                {
                    UserId = targetUser.Id,
                    TargetWeight = 72,
                    TargetSteps = 12000,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };

            context.Goals.AddRange(goals);
            await context.SaveChangesAsync();
        }

        if (!await context.UserActivities.AnyAsync(x => x.UserId == targetUser.Id))
        {
            var activityTypes = await context.ActivityTypes
                .AsNoTracking()
                .ToListAsync();

            float CaloriesFor(string name) =>
                activityTypes.First(x => x.Name == name).CaloriesPerHour;

            var activities = new List<UserActivity>
            {
                new()
                {
                    UserId = targetUser.Id,
                    ActivityTypeId = activityTypes.First(x => x.Name == "Running").Id,
                    DurationMinutes = 45,
                    CaloriesBurned = CalculateCaloriesBurned(CaloriesFor("Running"), 45),
                    ActivityDate = DateTime.UtcNow.AddDays(-1).AddHours(-2)
                },
                new()
                {
                    UserId = targetUser.Id,
                    ActivityTypeId = activityTypes.First(x => x.Name == "Walking").Id,
                    DurationMinutes = 60,
                    CaloriesBurned = CalculateCaloriesBurned(CaloriesFor("Walking"), 60),
                    ActivityDate = DateTime.UtcNow.AddDays(-2).AddHours(-1)
                },
                new()
                {
                    UserId = targetUser.Id,
                    ActivityTypeId = activityTypes.First(x => x.Name == "Cycling").Id,
                    DurationMinutes = 30,
                    CaloriesBurned = CalculateCaloriesBurned(CaloriesFor("Cycling"), 30),
                    ActivityDate = DateTime.UtcNow.AddDays(-4).AddHours(-3)
                },
                new()
                {
                    UserId = targetUser.Id,
                    ActivityTypeId = activityTypes.First(x => x.Name == "Swimming").Id,
                    DurationMinutes = 40,
                    CaloriesBurned = CalculateCaloriesBurned(CaloriesFor("Swimming"), 40),
                    ActivityDate = DateTime.UtcNow.AddDays(-6).AddHours(-2)
                }
            };

            context.UserActivities.AddRange(activities);
            await context.SaveChangesAsync();
        }

        if (!await context.Notifications.AnyAsync(x => x.UserId == targetUser.Id))
        {
            var notifications = new List<Notification>
            {
                new()
                {
                    UserId = targetUser.Id,
                    Message = "High blood pressure detected",
                    Type = NotificationType.Warning,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-5)
                },
                new()
                {
                    UserId = targetUser.Id,
                    Message = "Daily goal achieved",
                    Type = NotificationType.Info,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new()
                {
                    UserId = targetUser.Id,
                    Message = "Time to log your latest measurement",
                    Type = NotificationType.Warning,
                    IsRead = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();
        }
    }

    private static int CalculateCaloriesBurned(
        float caloriesPerHour,
        int durationMinutes)
    {
        return (int)Math.Round((caloriesPerHour / 60f) * durationMinutes);
    }
}