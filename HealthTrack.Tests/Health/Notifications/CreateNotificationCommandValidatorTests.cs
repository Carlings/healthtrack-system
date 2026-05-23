using HealthTrack.Application.Health.Notifications.Commands;
using HealthTrack.Application.Health.Notifications.Validators;
using HealthTrack.Domain.Entities;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.Notifications;

public sealed class CreateNotificationCommandValidatorTests
{
    private readonly CreateNotificationCommandValidator _validator = new();

    [Test]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var command = new CreateNotificationCommand(
            "High blood pressure detected",
            NotificationType.Warning);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenMessageIsEmpty_ShouldFail()
    {
        var command = new CreateNotificationCommand(
            string.Empty,
            NotificationType.Warning);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
    }
}