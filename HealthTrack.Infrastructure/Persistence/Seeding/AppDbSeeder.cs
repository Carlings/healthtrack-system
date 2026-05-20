using Bogus;
using HealthTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthTrack.Infrastructure.Persistence.Seeding;

public static class AppDbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        if (await context.Users.AnyAsync())
            return;

        Randomizer.Seed = new Random(42);
        var faker = new Faker("en");

        var activityTypes = new List<ActivityType>
        {
            new() { Name = "Running", CaloriesPerHour = 600 },
            new() { Name = "Walking", CaloriesPerHour = 250 },
            new() { Name = "Cycling", CaloriesPerHour = 500 },
            new() { Name = "Swimming", CaloriesPerHour = 650 }
        };

        context.ActivityTypes.AddRange(activityTypes);
        await context.SaveChangesAsync();

        var user = new User
        {
            Email = faker.Internet.Email().ToLowerInvariant(),
            PasswordHash = "DemoPassword123!",
            Name = faker.Name.FullName(),
            BirthDate = DateOnly.FromDateTime(faker.Date.Past(30, DateTime.Today.AddYears(-18))),
            Height = faker.Random.Int(160, 195),
            Gender = faker.PickRandom(Gender.Male, Gender.Female),
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var records = new Faker<HealthRecord>()
            .RuleFor(x => x.UserId, _ => user.Id)
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

        var goals = new List<Goal>
        {
            new()
            {
                UserId = user.Id,
                TargetWeight = 75,
                TargetSteps = 10000,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Goals.AddRange(goals);
        await context.SaveChangesAsync();
    }
}