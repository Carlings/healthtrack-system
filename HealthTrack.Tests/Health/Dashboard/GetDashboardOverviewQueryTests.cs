using HealthTrack.Application.Common.Interfaces.Identity;
using HealthTrack.Application.Common.Interfaces.Repositories;
using HealthTrack.Application.Health.Dashboard.Queries;
using HealthTrack.Domain.Entities;
using HealthTrack.Tests.Common;
using Moq;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.Dashboard;

public sealed class GetDashboardOverviewQueryTests
{
    private Mock<IHealthRecordRepository> _healthRecords = null!;
    private Mock<IGoalRepository> _goals = null!;
    private Mock<IUserActivityRepository> _activities = null!;
    private Mock<INotificationRepository> _notifications = null!;
    private Mock<ICurrentUserService> _currentUser = null!;

    private GetDashboardOverviewQueryHandler _handler = null!;

    [SetUp]
    public void SetUp()
    {
        _healthRecords = new Mock<IHealthRecordRepository>();
        _goals = new Mock<IGoalRepository>();
        _activities = new Mock<IUserActivityRepository>();
        _notifications = new Mock<INotificationRepository>();
        _currentUser = new Mock<ICurrentUserService>();

        _handler = new GetDashboardOverviewQueryHandler(
            _healthRecords.Object,
            _goals.Object,
            _activities.Object,
            _notifications.Object,
            _currentUser.Object);
    }

    [Test]
    public async Task Handle_WhenDataExists_ShouldBuildDashboardOverview()
    {
        const int userId = 2;
        _currentUser.Setup(x => x.UserId).Returns(userId);

        var healthRecords = new List<HealthRecord>
        {
            TestDataFactory.CreateHealthRecord(
                17,
                new DateTime(2026, 05, 22, 06, 18, 07, 467, DateTimeKind.Utc).AddTicks(981),
                57.805496f,
                87,
                36.864975f,
                114,
                72,
                9725,
                6.158427f,
                userId),

            TestDataFactory.CreateHealthRecord(
                13,
                new DateTime(2026, 05, 15, 07, 59, 42, 425, DateTimeKind.Utc).AddTicks(4045),
                82.8082f,
                67,
                36.7f,
                120,
                80,
                2353,
                7.0f,
                userId),

            TestDataFactory.CreateHealthRecord(
                12,
                new DateTime(2026, 05, 07, 08, 52, 34, 483, DateTimeKind.Utc).AddTicks(8708),
                64.263885f,
                71,
                36.8f,
                118,
                79,
                9613,
                6.5f,
                userId),

            TestDataFactory.CreateHealthRecord(
                11,
                new DateTime(2026, 05, 05, 17, 41, 05, 243, DateTimeKind.Utc).AddTicks(2771),
                76.86088f,
                71,
                36.5f,
                120,
                80,
                5000,
                7.1f,
                userId),

            TestDataFactory.CreateHealthRecord(
                10,
                new DateTime(2026, 05, 05, 13, 23, 28, 25, DateTimeKind.Utc).AddTicks(9662),
                100.158585f,
                71,
                36.5f,
                120,
                80,
                4500,
                7.2f,
                userId),

            TestDataFactory.CreateHealthRecord(
                9,
                new DateTime(2026, 05, 01, 17, 14, 06, 792, DateTimeKind.Utc).AddTicks(1084),
                63.12687f,
                70,
                36.5f,
                120,
                80,
                4000,
                6.9f,
                userId),
        };

        var goals = new List<Goal>
        {
            TestDataFactory.CreateGoal(
                7,
                userId,
                72f,
                12000,
                new DateTime(2026, 05, 19, 22, 15, 59, 533, DateTimeKind.Utc).AddTicks(8925))
        };

        var running = TestDataFactory.CreateActivityType(1, "Running", 600);
        var walking = TestDataFactory.CreateActivityType(2, "Walking", 250);
        var cycling = TestDataFactory.CreateActivityType(3, "Cycling", 500);

        var activities = new List<UserActivity>
        {
            TestDataFactory.CreateUserActivity(
                9,
                TestDataFactory.CreateUser(userId),
                running,
                45,
                450,
                new DateTime(2026, 05, 21, 20, 15, 59, 566, DateTimeKind.Utc).AddTicks(3154)),

            TestDataFactory.CreateUserActivity(
                10,
                TestDataFactory.CreateUser(userId),
                walking,
                60,
                250,
                new DateTime(2026, 05, 20, 21, 15, 59, 566, DateTimeKind.Utc).AddTicks(381)),

            TestDataFactory.CreateUserActivity(
                11,
                TestDataFactory.CreateUser(userId),
                cycling,
                30,
                250,
                new DateTime(2026, 05, 18, 19, 15, 59, 566, DateTimeKind.Utc).AddTicks(4043)),
        };

        var notifications = new List<Notification>
        {
            TestDataFactory.CreateNotification(
                8,
                TestDataFactory.CreateUser(userId),
                NotificationType.Critical,
                "Time to log your latest health record",
                false,
                new DateTime(2026, 05, 22, 22, 21, 24, 930, DateTimeKind.Utc).AddTicks(4528)),

            TestDataFactory.CreateNotification(
                7,
                TestDataFactory.CreateUser(userId),
                NotificationType.Warning,
                "High blood pressure detected",
                false,
                new DateTime(2026, 05, 22, 22, 21, 18, 287, DateTimeKind.Utc).AddTicks(3173)),

            TestDataFactory.CreateNotification(
                4,
                TestDataFactory.CreateUser(userId),
                NotificationType.Warning,
                "High blood pressure detected",
                false,
                new DateTime(2026, 05, 22, 17, 15, 59, 590, DateTimeKind.Utc).AddTicks(998)),

            TestDataFactory.CreateNotification(
                3,
                TestDataFactory.CreateUser(userId),
                NotificationType.Info,
                "Read notification",
                true,
                new DateTime(2026, 05, 20, 17, 15, 59, 590, DateTimeKind.Utc).AddTicks(998)),
        };

        _healthRecords
            .Setup(x => x.GetByUserIdAsync(userId, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(healthRecords);

        _goals
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(goals);

        _activities
            .Setup(x => x.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activities);

        _notifications
            .Setup(x => x.GetUnreadByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(notifications.Where(x => !x.IsRead).ToList());

        var result = await _handler.Handle(new GetDashboardOverviewQuery(), CancellationToken.None);

        Assert.That(result.Stats, Is.Not.Null);
        Assert.That(result.Stats!.Weight, Is.EqualTo(57.805496f).Within(0.0001f));
        Assert.That(result.Stats.Pulse, Is.EqualTo(87));

        Assert.That(result.Goal, Is.Not.Null);
        Assert.That(result.Goal!.Id, Is.EqualTo(7));
        Assert.That(result.Goal.TargetWeight, Is.EqualTo(72f));
        Assert.That(result.Goal.TargetSteps, Is.EqualTo(12000));
        Assert.That(result.Goal.WeightProgressPercentRaw, Is.EqualTo(231.33f).Within(0.01f));
        Assert.That(result.Goal.WeightProgressPercentClamped, Is.EqualTo(100f));
        Assert.That(result.Goal.StepsProgressPercent, Is.EqualTo(81.04f).Within(0.01f));
        Assert.That(result.Goal.CurrentWeight, Is.EqualTo(57.805496f).Within(0.0001f));
        Assert.That(result.Goal.CurrentSteps, Is.EqualTo(9725));
        Assert.That(result.Goal.WeightGap, Is.EqualTo(-14.19f).Within(0.05f));
        Assert.That(result.Goal.StepsGap, Is.EqualTo(2275));

        Assert.That(result.WeightTrend.Count, Is.EqualTo(6));
        Assert.That(result.WeightTrend[0].Weight, Is.EqualTo(63.12687f).Within(0.0001f));
        Assert.That(result.WeightTrend[5].Weight, Is.EqualTo(57.805496f).Within(0.0001f));

        Assert.That(result.LatestRecords.Count, Is.EqualTo(3));
        Assert.That(result.LatestRecords[0].Id, Is.EqualTo(17));
        Assert.That(result.LatestRecords[1].Id, Is.EqualTo(13));
        Assert.That(result.LatestRecords[2].Id, Is.EqualTo(12));

        Assert.That(result.LatestActivities.Count, Is.EqualTo(3));
        Assert.That(result.LatestActivities[0].ActivityType, Is.EqualTo("Running"));
        Assert.That(result.LatestActivities[1].ActivityType, Is.EqualTo("Walking"));
        Assert.That(result.LatestActivities[2].ActivityType, Is.EqualTo("Cycling"));

        Assert.That(result.LatestNotifications.Count, Is.EqualTo(3));
        Assert.That(result.UnreadNotificationsCount, Is.EqualTo(3));
    }
}