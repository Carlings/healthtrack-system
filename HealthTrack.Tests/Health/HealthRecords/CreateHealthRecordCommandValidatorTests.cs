using HealthTrack.Application.Health.HealthRecords.Commands;
using HealthTrack.Application.Health.HealthRecords.Validators;
using NUnit.Framework;

namespace HealthTrack.Tests.Health.HealthRecords;

public sealed class CreateHealthRecordCommandValidatorTests
{
    private readonly CreateHealthRecordCommandValidator _validator = new();

    [Test]
    public void Validate_WhenCommandIsValid_ShouldBeValid()
    {
        var command = new CreateHealthRecordCommand(
            new DateTime(2026, 05, 22, 10, 30, 00, DateTimeKind.Utc),
            78.5f,
            72,
            36.6f,
            120,
            80,
            8500,
            7.5f);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_WhenWeightIsInvalid_ShouldFail()
    {
        var command = new CreateHealthRecordCommand(
            new DateTime(2026, 05, 22, 10, 30, 00, DateTimeKind.Utc),
            0,
            72,
            36.6f,
            120,
            80,
            8500,
            7.5f);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Exists(x => x.PropertyName == nameof(CreateHealthRecordCommand.Weight)), Is.True);
    }

    [Test]
    public void Validate_WhenPulseIsInvalid_ShouldFail()
    {
        var command = new CreateHealthRecordCommand(
            new DateTime(2026, 05, 22, 10, 30, 00, DateTimeKind.Utc),
            78.5f,
            500,
            36.6f,
            120,
            80,
            8500,
            7.5f);

        var result = _validator.Validate(command);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Exists(x => x.PropertyName == nameof(CreateHealthRecordCommand.Pulse)), Is.True);
    }
}