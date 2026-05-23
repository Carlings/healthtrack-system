using HealthTrack.Application.Health.Activities.Commands;
using HealthTrack.Application.Health.Activities.Validators;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class CreateActivityCommandValidatorTests
{
    private readonly CreateActivityCommandValidator _validator = new();

    [Test]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var command = new CreateActivityCommand(
            1,
            45,
            new DateTime(2026, 05, 22, 18, 00, 00, DateTimeKind.Utc));

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenDurationIsInvalid_ShouldFail()
    {
        var command = new CreateActivityCommand(
            1,
            -5,
            new DateTime(2026, 05, 22, 18, 00, 00, DateTimeKind.Utc));

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
    }
}